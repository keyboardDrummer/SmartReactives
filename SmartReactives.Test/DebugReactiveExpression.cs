using System;
using SmartReactives.Common;

namespace SmartReactives.Test
{
    public class DebugReactiveExpression<T> : ReactiveExpression<T>
    {
        readonly string name;

        public DebugReactiveExpression(Func<T> expression, string name = null) : base(expression)
        {
            this.name = name;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return name ?? "unnamed";
        }
    }
}