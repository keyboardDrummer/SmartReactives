using System;
using System.Collections.Specialized;
using System.Reactive;
using System.Reactive.Linq;

namespace SmartReactives.List
{
    /// <summary>
    /// Turns a <see cref="INotifyCollectionChanged"/> into an <see cref="IObservable{T}"/>
    /// </summary>
    public class NotifyCollectionChangedAsObservable : IObservable<EventPattern<NotifyCollectionChangedEventArgs>>
    {
        private readonly IObservable<EventPattern<NotifyCollectionChangedEventArgs>> _observable;
        private readonly INotifyCollectionChanged _observableCollection;

        /// <inheritdoc/>
        public NotifyCollectionChangedAsObservable(INotifyCollectionChanged observableCollection)
        {
            _observableCollection = observableCollection;
            _observable = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                handler => _observableCollection.CollectionChanged += handler,
                handler => _observableCollection.CollectionChanged -= handler);
        }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<EventPattern<NotifyCollectionChangedEventArgs>> observer)
        {
            return _observable.Subscribe(observer);
        }
    }
}