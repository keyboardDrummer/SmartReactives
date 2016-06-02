using System;
using System.Collections.Generic;

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

        public static IList<T> ToReactive<T>(this IList<T> original)
        {
            return new ReactiveList<T>(original);
        }

        public static ISet<T> ToReactive<T>(this ISet<T> original)
        {
            return new ReactiveSet<T>(original);
        }

        public static IDictionary<TKey, TValue> ToReactive<TKey, TValue>(this IDictionary<TKey, TValue> original)
        {
            return new ReactiveDictionary<TKey, TValue>(original);
        }
    }
}