using System;

namespace SmartReactives.Common
{
    /// <summary>
    /// Contains factory methods for reactive objects.
    /// </summary>
    public static class Reactive
    {
        public static ReactiveVariable<T> Variable<T>(T value)
        {
            return new ReactiveVariable<T>(value);
        }

        public static ReactiveExpression<T> Expression<T>(Func<T> expression)
        {
            return new ReactiveExpression<T>(expression);
        }

        public static ReactiveCache<T> Cache<T>(Func<T> expression)
        {
            return new ReactiveCache<T>(expression);
        }
    }
}