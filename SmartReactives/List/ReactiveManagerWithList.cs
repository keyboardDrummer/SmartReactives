using System;
using System.Collections.Specialized;
using System.Reactive;
using System.Runtime.CompilerServices;
using SmartReactives.Core;

namespace SmartReactives.List
{
    /// <summary>
    /// Adds reactive functionality to non-reactive lists as long as they implement <see cref="INotifyCollectionChanged"/>.
    /// The <see cref="INotifyCollectionChanged"/> object becomes a node in the dependency graph.
    /// Care is taken so that only on handler is attached to the CollectionChanged event of each <see cref="INotifyCollectionChanged"/>.
    /// </summary>
    public static class ReactiveManagerWithList
    {
        private static readonly ConditionalWeakTable<INotifyCollectionChanged, IDisposable> _subscriptions =
            new ConditionalWeakTable<INotifyCollectionChanged, IDisposable>();

        /// <summary>
        /// Evaluate a function that returns an <see cref="INotifyCollectionChanged"/>
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
        private static void RegisterChanges<T>(T result)
            where T : INotifyCollectionChanged
        {
            IDisposable disposable;
            if (!_subscriptions.TryGetValue(result, out disposable))
            {
                var wrapper = new NotifyCollectionChangedAsObservable(result);
                var subscription = wrapper.Subscribe(CollectionChangedHandler);
                _subscriptions.Add(result, subscription);
            }
        }

        private static void CollectionChangedHandler(EventPattern<NotifyCollectionChangedEventArgs> eventPattern)
        {
            ReactiveManager.WasChanged(eventPattern.Sender);
        }
    }
}