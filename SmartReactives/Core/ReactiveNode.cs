using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SmartReactives.Core
{
	/// <summary>
    /// A node in the dependency graph of <see cref="ReactiveManager"/>
    /// </summary>
	class ReactiveNode
	{
	    long notificationsHad;
		Chain<Dependency> dependencies; //Since this is a write often read once scenario, we use a singly linked list instead of an array.

        public IList<Dependency> GetCopy()
		{
			lock (this)
			{
				var result = new List<Dependency>();
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

		public void NotifyChildren(IList<IListener> result)
        {
            Chain<Dependency> current;
            lock (this)
            {
                current = dependencies;
                dependencies = null; //Cleans up memory but not required for semantics since the notification counter makes sure DependentReferences never trigger two notifications.
            }

			while (current != null)
			{
				WasChangedForChild(result, current.Value);
				current = current.Next;
			}
        }

	    static void WasChangedForChild(IList<IListener> result, Dependency childEdge)
		{
			var child = childEdge.Value;
			if (child == null)
				return;

			var childNode = ReactiveManager.GetNode(child);
			if (childNode.notificationsHad == childEdge.NotificationsHad) //Determines if the edge is still up-to-date.
			{
				childNode.WasNotified();
				result.Add(child);
                childNode.NotifyChildren(result);
            }
		}

		internal void Add(Dependency element)
		{
			lock(this)
			{
				dependencies = new Chain<Dependency>(element, dependencies);
			}
		}

		public Dependency GetDependency(IListener dependent)
		{
			return new Dependency(notificationsHad, dependent);
		}
	}
}