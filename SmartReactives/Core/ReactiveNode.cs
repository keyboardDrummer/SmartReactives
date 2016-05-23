using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SmartReactives.Core
{
	class ReactiveNode
	{
	    long notificationsHad;
	    IList<DependentReference> data = new List<DependentReference>();

        public IList<DependentReference> GetCopy()
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

		public void NotifyChildren()
        {
            IList<DependentReference> swap;
            lock (this)
            {
                swap = data;
                data = new List<DependentReference>(swap.Count); //Useful for performance, but not required since the notification counter makes sure DependentReferences never trigger two notifications.
            }
            int count = swap.Count;

            for (int index = 0; index < count; index++)
            {
                var value = swap[index];
                WasChangedForChild(value);
            }
        }

	    static void WasChangedForChild(DependentReference childReference)
		{
			var child = childReference.Value;
			if (child == null)
				return;

			var stampedChildList = ReactiveManager.GetNode(child);
			if (stampedChildList.notificationsHad == childReference.NotificationsHad)
			{
				stampedChildList.WasNotified();
                child.Notify();
                stampedChildList.NotifyChildren();
            }
		}

		internal void Add(DependentReference element)
		{
			lock(this)
			{
				data.Add(element);
			}
		}

		public DependentReference GetReference(IListener dependent)
		{
			return new DependentReference(notificationsHad, dependent);
		}
	}
}