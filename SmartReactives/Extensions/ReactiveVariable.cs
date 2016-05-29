using System.Collections.Generic;
using SmartReactives.Core;

namespace SmartReactives.Extensions
{
	/// <summary>
	/// A variable which can be used in an <see cref="ReactiveExpression{T}"/> and other reactive objects such as <see cref="ReactiveCache{T}"/>
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
		/// Set a new value, but only raise a change if the new value does not equal the existing one.
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
		internal IEnumerable<object> Dependents => ReactiveManager.GetDependents(this);
	}
}
