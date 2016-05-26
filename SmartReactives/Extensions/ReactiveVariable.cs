using System.Collections.Generic;
using SmartReactives.Core;

namespace SmartReactives.Extensions
{
	/// <summary>
	/// A variable which can be used in expressions using automatic change tracking.
	/// </summary>
	public class ReactiveVariable<T>
	{
		T value;

		public ReactiveVariable(T value = default(T))
		{
			this.value = value;
		}

		/// <summary>
		/// The value
		/// </summary>
		public T Value
		{
			get
			{
				ReactiveManager.WasRead(this);
				return value;
			}
			set
			{
				this.value = value;
				ReactiveManager.WasChanged(this);
			}
		}

		/// <summary>
		/// Only raises changes if the new value is not Equal to the existing one.
		/// </summary>
		public void SetValueIfChanged(T newValue)
		{
			if (!Equals(Value, newValue))
			{
				Value = newValue;
			}
		}

		// ReSharper disable once UnusedMember.Local
		/// <summary>
		/// For debugging purposes.
		/// </summary>
		public IEnumerable<object> Dependents => ReactiveManager.GetDependents(this);
	}
}
