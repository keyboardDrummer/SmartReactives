using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using SmartReactives.Core;

namespace SmartReactives.List
{
    /// <summary>
    /// Adds reactive functionality to non-reactive lists as long as they implement <see cref="INotifyCollectionChanged" />.
    /// The <see cref="INotifyCollectionChanged" /> object becomes a node in the dependency graph.
    /// Care is taken so that only on handler is attached to the CollectionChanged event of each <see cref="INotifyCollectionChanged" />.
    /// </summary>
    public static class ReactiveManagerWithList
    {
        static readonly ConditionalWeakTable<INotifyCollectionChanged, object> subscriptions =
            new ConditionalWeakTable<INotifyCollectionChanged, object>();

	    static readonly object value = new object();

        /// <summary>
        /// Evaluate a function that returns an <see cref="INotifyCollectionChanged" />
        /// </summary>
        public static T Evaluate<T>(Func<T> evaluate) //TODO zeker weten dat we de registratie aan de lijst willen hangen?
            where T : INotifyCollectionChanged
        {
            var state = ReactiveManager.State;
            if (!state.Enabled)
            {
                return evaluate();
            }

            var result = evaluate();
            if (result != null)
            {
                state.WasRead(result);
                RegisterChanges(result);
            }
            return result;
        }

        //We could also do this on the set instead of the get.
        static void RegisterChanges<T>(T result)
            where T : INotifyCollectionChanged
        {
            object useless;
            if (!subscriptions.TryGetValue(result, out useless))
            {
	            result.CollectionChanged += OnResultOnCollectionChanged;
                subscriptions.Add(result, value);
            }
        }

	    static void OnResultOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
	    {
		    ReactiveManager.WasChanged(sender);
	    }
    }
}