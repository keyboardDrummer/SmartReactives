using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using SmartReactives.Core;

namespace SmartReactives.Extensions
{
	/// <summary>
	/// Captures an expression whose value may change over time.
	/// The different values the expression has over time are exposed as an <see cref="IObservable{T}"/>
	/// Make sure the provided expression only changes its value due to changes in reactive variables that it depends on.
	/// </summary>
	[Serializable]
	public class ReactiveExpression<T> : IObservable<Func<T>>, IListener
	{
	    readonly Func<T> func;
	    readonly object name;
	    readonly ISubject<Func<T>> subject = new Subject<Func<T>>();

		/// <summary>
		/// The debug object can be passed to override the toString method.
		/// </summary>
		public ReactiveExpression(Func<T> func, object name = null)
		{
			this.func = func;
			this.name = name;
		}

		/// <summary>
		/// Evaluate the function and return its value.
		/// </summary>
		public T Evaluate()
		{
			return ReactiveManager.Evaluate(this, func);
		}

		public void Notify()
		{
			subject.OnNext(Evaluate);
		}

		public IDisposable Subscribe(IObserver<Func<T>> observer)
		{
            observer.OnNext(Evaluate);
			return subject.Subscribe(observer);
		}

		// ReSharper disable once UnusedMember.Local
		/// <summary>
		/// For debugging purposes.
		/// </summary>
		public IEnumerable<object> Dependents => ReactiveManager.GetDependents(this);

		/// <inheritdoc/>
		public override string ToString()
		{
			return name?.ToString() ?? "unnamed";
		}
	}
}
