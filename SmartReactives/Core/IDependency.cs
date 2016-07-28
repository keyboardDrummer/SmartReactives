namespace SmartReactives.Core
{
	interface IDependency
	{
		long NotificationsHad { get; }
		IListener Value { get; }
	}
}