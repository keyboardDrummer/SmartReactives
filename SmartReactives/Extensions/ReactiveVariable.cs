using System.Collections.Generic;
using SmartReactives.Core;

namespace SmartReactives.Extensions
{
	/// <summary>
	/// A variable which can be used in expressions using automatic change tracking.
	/// </summary>
	public class ReactiveVariable<T>
	{
		private T _value;

		/// <summary>
		/// The value
		/// </summary>
		public T Value
		{
			get
			{
				ReactiveManager.WasRead(this);
				return _value;
			}
			set
			{
				_value = value;
				ReactiveManager.WasChanged(this);
			}
		}

		/// <summary>
		/// Only raises changes if the new value is not Equal to the existing one.
		/// </summary>
		public void SetValueIfChanged(T value)
		{
			if (!Equals(Value, value))
			{
				Value = value;
			}
		}

		// ReSharper disable once UnusedMember.Local
		/// <summary>
		/// For debugging purposes.
		/// </summary>
		public IEnumerable<object> Dependents => ReactiveManager.GetDependents(this);
	}
}
