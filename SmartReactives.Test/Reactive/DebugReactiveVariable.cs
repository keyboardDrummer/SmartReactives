using SmartReactives.Extensions;

namespace SmartReactives.Test.Reactive
{
	class DebugReactiveVariable<T> : ReactiveVariable<T>
	{
		private readonly object _debugObj;

		public DebugReactiveVariable(object debugObj)
		{
			_debugObj = debugObj;
		}

		public override string ToString()
		{
			return _debugObj + "";
		}
	}
}