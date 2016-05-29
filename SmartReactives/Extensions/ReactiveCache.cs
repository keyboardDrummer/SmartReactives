using System;
using System.Collections.Generic;
using SmartReactives.Core;

namespace SmartReactives.Extensions
{
	/// <summary>
	/// A cache based on the <see cref="ReactiveManager"/> framework.
	/// This cache will automagically invalidate when its function changes based on reactive variables.
	/// </summary>
	public class ReactiveCache<T> : IListener
	{
	    readonly Func<T> get;
		T lastValue;

		/// <summary>
		/// Pass the expression that you want to cache.
		/// </summary>
		public ReactiveCache(Func<T> get)
		{
			this.get = get;
		}

		/// <summary>
		/// Get a value from the cache. 
		/// </summary>
		public T Get()
		{
			if (IsSet)
			{
				ReactiveManager.WasRead(this);
			}
			else
			{
				lastValue = ReactiveManager.Evaluate(this, get);
				IsSet = true;
			}
			return lastValue;
		}

		public bool IsSet
		{
			get;
			private set;
		}

		void IListener.Notify()
		{
			IsSet = false;
		}

		/// <summary>
		/// Invalidate the cache manually. Usefull for cases not covered by the Reactive framework.
		/// </summary>
		public void Invalidate()
		{
			IsSet = false;
			ReactiveManager.WasChanged(this);
		}

		/// <summary>
		/// Useful for debugging.
		/// </summary>
		// ReSharper disable once UnusedMember.Local
		internal IEnumerable<object> Dependents => ReactiveManager.GetDependents(this);
	}
}