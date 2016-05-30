using System.Collections;
using System.Collections.Generic;

namespace SmartReactives.Core
{
	/// <summary>
	/// A minimal singly linked list.
	/// </summary>
	class Chain<T>
	{
		public Chain(T value, Chain<T> next)
		{
			Value = value;
			Next = next;
		}

		public T Value { get; }

		public Chain<T> Next { get; }
	}
}