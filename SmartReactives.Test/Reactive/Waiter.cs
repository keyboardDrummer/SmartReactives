using System.Threading;

namespace SmartReactives.Test.Reactive
{
	class Waiter
	{
		private bool _waiting = true;

		public void Wait()
		{
			while (_waiting)
			{
				Thread.Sleep(10);
			}
		}

		public void Release()
		{
			_waiting = false;
		}
	}
}
