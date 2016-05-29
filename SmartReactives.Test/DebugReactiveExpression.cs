using System;
using SmartReactives.Extensions;

namespace SmartReactives.Test
{
    public class DebugReactiveExpression<T> : ReactiveExpression<T>
    {
        readonly string name;

        public DebugReactiveExpression(Func<T> func, string name = null) : base(func)
        {
            this.name = name;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return name ?? "unnamed";
        }
    }
}