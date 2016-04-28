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
	public class ObservableExpression<T> : IObservable<T>, IListener
	{
		private readonly Func<T> _func;
		private readonly object _name;
		private readonly ISubject<T> _subject = new Subject<T>();

		/// <summary>
		/// The debug object can be passed to override the toString method.
		/// </summary>
		public ObservableExpression(Func<T> func, object name = null)
		{
			_func = func;
			_name = name;
		}

		/// <summary>
		/// Evaluate the function and return its value.
		/// </summary>
		public T Evaluate()
		{
			return ReactiveManager.Evaluate(this, _func);
		}

		public void Notify()
		{
			_subject.OnNext(default(T)); //TODO fixxx!!! this now leads to infinite loopy shit.
		}

		/// <inheritdoc/>
		public IDisposable Subscribe(IObserver<T> observer)
		{
			return _subject.Subscribe(observer);
		}

		// ReSharper disable once UnusedMember.Local
		/// <summary>
		/// For debugging purposes.
		/// </summary>
		public IEnumerable<object> Dependents => ReactiveManager.GetDependents(this);

		/// <inheritdoc/>
		public override string ToString()
		{
			return _name?.ToString() ?? "unnamed";
		}
	}
}
