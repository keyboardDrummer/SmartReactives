namespace SmartReactives.Core
{
	/// <summary>
	/// A strong edge in the dependency graph of <see cref="ReactiveManager"/>.
	/// </summary>
	class StrongDependency : IDependency
	{
		public StrongDependency(long notificationsHad, IListener value)
		{
			NotificationsHad = notificationsHad;
			Value = value;
		}

		public long NotificationsHad { get; }
		public IListener Value { get; }

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}