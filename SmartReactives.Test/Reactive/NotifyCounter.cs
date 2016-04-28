using SmartReactives.Core;

namespace SmartReactives.Test.Reactive
{
	class NotifyCounter : IListener
	{
		public int Counter { get; set; }

		public void Notify()
		{
			Counter++;
		}
	}
}