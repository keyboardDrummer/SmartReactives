using System;

namespace SmartReactives.Core
{
	class DependentReference
	{
		public long NotificationsHad
		{
			get;
		}

	    readonly WeakReference nodeReference;
		public IListener Value => nodeReference.Target as IListener;

		public DependentReference(long notificationsHad, IListener node)
		{
			NotificationsHad = notificationsHad;
			nodeReference = new WeakReference(node);
		}

	    public override string ToString()
	    {
	        return Value?.ToString() ?? "empty reference";
	    }
	}
}