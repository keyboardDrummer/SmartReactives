using System;

namespace SmartReactives.Core
{
	class DependentReference
	{
		public long NotificationsHad
		{
			get;
		}

		private readonly WeakReference _nodeReference;
		public IListener Value => _nodeReference.Target as IListener;

		public DependentReference(long notificationsHad, IListener node)
		{
			NotificationsHad = notificationsHad;
			_nodeReference = new WeakReference(node);
		}
	}
}