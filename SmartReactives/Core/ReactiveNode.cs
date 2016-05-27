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
	    IList<Dependency> data = new List<Dependency>();

        public IList<Dependency> GetCopy()
		{
			lock (this)
			{
				return data.ToList();
			}
		}

		public long WasNotified()
		{
			return Interlocked.Increment(ref notificationsHad);
		}

		public void NotifyChildren(IList<IListener> result)
        {
            IList<Dependency> swap;
            lock (this)
            {
                swap = data;
                data = new List<Dependency>(swap.Count); //Useful for performance, but not required since the notification counter makes sure DependentReferences never trigger two notifications.
            }
            int count = swap.Count;

            for (int index = 0; index < count; index++)
            {
                var value = swap[index];
                WasChangedForChild(result, value);
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
				data.Add(element);
			}
		}

		public Dependency GetDependency(IListener dependent)
		{
			return new Dependency(notificationsHad, dependent);
		}
	}
}