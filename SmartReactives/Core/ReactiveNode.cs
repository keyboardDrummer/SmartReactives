using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SmartReactives.Core
{
	class ReactiveNode
	{
		private long _notificationsHad;
		private readonly IList<DependentReference> _data = new List<DependentReference>();

		public IList<DependentReference> GetCopy()
		{
			lock (this)
			{
				return _data.ToList();
			}
		}

		public long WasNotified()
		{
			return Interlocked.Increment(ref _notificationsHad);
		}

		public void NotifyChildren()
		{
			lock(this) //TODO potentieel duurt deze lock wel lang maar het scheelt zo wel een nieuwe lijst maken.. Dit is ook niet zo thread-safe omdat je thread 1 op 2 laat wachten terwijl je externe code op 2 aanroept..
			{
				int count = _data.Count;
				for (int index = 0; index < count; index++)
				{
					var value = _data[index];
					WasChangedForChild(value);
				}
				_data.Clear();
			}
		}

		private static void WasChangedForChild(DependentReference childReference)
		{
			var child = childReference.Value;
			if (child == null)
				return;

			var stampedChildList = ReactiveManager.GetNode(child);
			if (stampedChildList._notificationsHad == childReference.NotificationsHad)
			{
				stampedChildList.WasNotified();
				child.Notify();
				stampedChildList.NotifyChildren();
			}
		}

		internal void Add(DependentReference element)
		{
			lock(this) //TODO deze lock moet eigenlijk niet conflicten met de lock in NotifyChildren. Is dat voldoende? Zou iig wel kunnen met een linked list circle denk ik.
			{
				_data.Add(element);
			}
		}

		public DependentReference GetReference(IListener dependent)
		{
			return new DependentReference(_notificationsHad, dependent);
		}
	}
}