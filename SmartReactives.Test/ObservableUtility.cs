using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;

namespace SmartReactives.Test
{
	/// <summary>
	/// Utility functions related to <see cref="IObservable{T}" />
	/// </summary>
	public static class ObservableUtility
	{
		/// <summary>
		/// Creates an <see cref="IObservable{T}" /> from a property with <see cref="INotifyPropertyChanged" />
		/// </summary>
		public static IObservable<T> FromProperty<T>(Expression<Func<T>> getProperty, bool observeInitialValue = true)
		{
			var memberExpression = getProperty.Body as MemberExpression;
			if (memberExpression == null)
			{
				throw new ArgumentException("Passed Expression must be a MemberExpression");
			}
			var property = memberExpression.Member as PropertyInfo;
			if (property == null)
			{
				throw new ArgumentException("Passed Expression must be end in a property access");
			}
			var container = Expression.Lambda(memberExpression.Expression).Compile().DynamicInvoke() as INotifyPropertyChanged;
			if (container == null)
			{
				throw new ArgumentException("Property container must be a INotifyPropertyChanged");
			}

			return CreateObservable<T>(property, container, observeInitialValue);
		}

		static IObservable<T> CreateObservable<T>(PropertyInfo property, INotifyPropertyChanged container, bool observeInitialValue = true)
		{
			return Observable.Create<T>(observer =>
			{
				PropertyChangedEventHandler handler = (sender, args) =>
				{
					if (args.PropertyName == property.Name)
					{
						observer.OnNext((T) property.GetValue(container));
					}
				};
				container.PropertyChanged += handler;
				if (observeInitialValue)
				{
					observer.OnNext((T) property.GetValue(container));
				}
				return () => container.PropertyChanged -= handler;
			});
		}
	}
}