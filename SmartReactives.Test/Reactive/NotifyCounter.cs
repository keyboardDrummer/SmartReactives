using SmartReactives.Core;

namespace SmartReactives.Test.Reactive
{
	class NotifyCounter : IListener
	{
		private int _counter;

		public int Counter
		{
			get
			{
				return _counter;
			}
		}

		public void Notify()
		{
			_counter++;
		}
	}
}
