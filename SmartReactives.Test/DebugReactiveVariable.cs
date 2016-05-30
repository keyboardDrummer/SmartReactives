using SmartReactives.Common;

namespace SmartReactives.Test
{
	class DebugReactiveVariable<T> : ReactiveVariable<T>
	{
		readonly object debugObj;

		public DebugReactiveVariable(T value, object debugObj) : base(value)
		{
			this.debugObj = debugObj;
		}

		public override string ToString()
		{
			return debugObj + "";
		}
	}
}