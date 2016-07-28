using System;

namespace SmartReactives.Core
{
	/// <summary>
	/// An edge in the dependency graph of <see cref="ReactiveManager"/>.
	/// </summary>
	class WeakStrongDependency : IDependency
	{
		public long NotificationsHad { get; }

		readonly WeakReference weakReference;
		private readonly object strong;

		public IListener Value
		{
			get
			{
				var weakValue = weakReference.Target;
				if (weakValue == null)
					return null;
				return new WeakStrongReactive(weakValue, strong);
			}
		}

		public WeakStrongDependency(long notificationsHad, WeakStrongReactive dependent)
		{
			NotificationsHad = notificationsHad;
			weakReference = new WeakReference(dependent.Weak);
			strong = dependent.Strong;
		}

		public override string ToString()
		{
			return Value?.ToString() ?? "empty reference";
		}
	}
}