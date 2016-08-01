using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SmartReactives.Core
{
    /// <summary>
    /// A node in the dependency graph of <see cref="ReactiveManager"/>
    /// We define forward in the dependency graph as moving from dependencies to dependents.
    /// So given that a node is changed, we can find all effected dependents by traversing forward through the graph from the changed node.
    /// After notifying a node, we must delete all incoming edges of this node to prevent false notifications.
    /// 
    /// The graph is designed with speed in mind. These properties are important to consider:
    /// 1) We only traverse forwards.
    /// 2) Every edge is only traversed once.
    /// 3) We often add an incoming edge.
    /// 4) We often want to delete all incoming edges of a node.
    /// 
    /// Because we often add outgoing edges but only traverse them once, we store a linked list of successors for each node.
    /// The linked list is an appropriate situation for a write often read once situation.
    /// 
    /// We store a sort of timestamp in the field notificationsHad both on the nodes and on the edges in the graph.
    /// While traversing the graph, we skip edges whose timestamp does not match of their target node, as if these edges were not part of the graph. 
    /// Incrementing the timestamp of a node is thus a cheap way to effectively delete all incoming edges of this node. 
    /// </summary>
    class ReactiveNode
    {
        long notificationsHad;
        Chain<IDependency> dependencies; //Since this is a write often read once scenario, we use a singly linked list instead of an array.

        public IList<IDependency> GetCopy()
        {
            lock (this)
            {
                var result = new List<IDependency>();
                var current = dependencies;
                while (current != null)
                {
                    result.Add(current.Value);
                    current = current.Next;
                }
                return result;
            }
        }

        public long WasNotified()
        {
            return Interlocked.Increment(ref notificationsHad);
        }

        public void NotifyChildren(ref Chain<IListener> result)
        {
            Chain<IDependency> current;
            lock (this)
            {
                current = dependencies;
                dependencies = null; //Cleans up memory but not required for semantics since the notification counter makes sure DependentReferences never trigger two notifications.
            }

            while (current != null)
            {
                WasChangedForChild(ref result, current.Value);
                current = current.Next;
            }
        }

        static void WasChangedForChild(ref Chain<IListener> result, IDependency childEdge)
        {
            var child = childEdge.Value;
            if (child == null)
                return;

            var childNode = ReactiveManager.GetNode(child);
            if (childNode.notificationsHad == childEdge.NotificationsHad) //Determines if the edge is still up-to-date.
            {
                childNode.WasNotified();
	            result = new Chain<IListener>(child, result);
                childNode.NotifyChildren(ref result);
            }
        }

        internal void Add(IDependency element)
        {
            lock (this)
            {
				dependencies = new Chain<IDependency>(element, dependencies);
            }
        }

        public IDependency GetDependency(IListener dependent)
        {
            return dependent is CompositeReactiveObject 
				? new WeakStrongDependency(notificationsHad, (CompositeReactiveObject)dependent) 
				: (IDependency)new WeakDependency(notificationsHad, dependent);
        }
    }
}