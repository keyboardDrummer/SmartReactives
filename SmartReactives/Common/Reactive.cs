using System;
using System.Collections.Generic;

namespace SmartReactives.Common
{
    /// <summary>
    /// This class exposes everything from the Common API of SmartReactives. 
    /// Use this class as a starting point for using SmartReactives.
    /// </summary>
    public static class Reactive
    {
		/// <summary>
		/// Create a reactive variable. All mutating variables used in reactive expressions should be reactive.
		/// </summary>
        public static ReactiveVariable<T> Variable<T>(T value)
        {
            return new ReactiveVariable<T>(value);
        }

		/// <summary>
		/// Creates a reactive expression. This is an expression that tracks when its value changes, due to changes in underlying inputs.
		/// You can subscribe to these changes through the IObservable interface which ReactiveExpression implements.
		/// </summary>
        public static ReactiveExpression<T> Expression<T>(Func<T> expression)
        {
            return new ReactiveExpression<T>(expression);
        }

		/// <summary>
		/// Creates a reactive cache. This is a cache that automatically clears itself when it becomes stale.
		/// </summary>
        public static ReactiveCache<T> Cache<T>(Func<T> expression)
        {
            return new ReactiveCache<T>(expression);
        }

		/// <summary>
		/// Converts a regular list into a reactive list, which can be used in reactive expressions.
		/// </summary>
        public static IList<T> ToReactive<T>(this IList<T> original)
        {
            return new ReactiveList<T>(original);
        }

		/// <summary>
		/// Converts a regular set into a reactive set, which can be used in reactive expressions.
		/// </summary>
		public static ISet<T> ToReactive<T>(this ISet<T> original)
        {
            return new ReactiveSet<T>(original);
        }

		/// <summary>
		/// Converts a regular dictionary into a reactive dictionary, which can be used in reactive expressions.
		/// </summary>
		public static IDictionary<TKey, TValue> ToReactive<TKey, TValue>(this IDictionary<TKey, TValue> original)
        {
            return new ReactiveDictionary<TKey, TValue>(original);
        }
    }
}