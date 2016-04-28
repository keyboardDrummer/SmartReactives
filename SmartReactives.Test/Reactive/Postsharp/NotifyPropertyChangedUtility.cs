using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace SmartReactives.Test.Reactive.Postsharp
{
	public static class NotifyPropertyChangedUtility
	{
		public static IObservable<PropertyChangedEventArgs> FromProperty(this INotifyPropertyChanged container, string propertyName)
		{
			return Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
				a => container.PropertyChanged += a,
				a => container.PropertyChanged -= a).Where(args => args.PropertyName == propertyName);
		}
	}
}