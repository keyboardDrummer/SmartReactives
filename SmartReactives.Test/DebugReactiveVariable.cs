using SmartReactives.Extensions;

namespace SmartReactives.Test
{
	class DebugReactiveVariable<T> : ReactiveVariable<T>
	{
		readonly object debugObj;

		public DebugReactiveVariable(object debugObj)
		{
			this.debugObj = debugObj;
		}

		public override string ToString()
		{
			return debugObj + "";
		}
	}
}