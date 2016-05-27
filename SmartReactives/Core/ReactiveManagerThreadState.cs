using System;
using System.Collections.Generic;

namespace SmartReactives.Core
{
    /// <summary>
    /// The thread specific state of <see cref="ReactiveManager"/>
    /// </summary>
    class ReactiveManagerThreadState
	{
	    Stack<Dependency> DependentsEvaluating
		{
			get;
		} = new Stack<Dependency>();

		public bool Enabled
		{
			get;
			set;
		} = true;

		IList<IListener> notifyList = new List<IListener>();
		/// <summary>
		/// Must be called whenever a node value is changed externally, for example when a property node's backing field changes its value.
		/// </summary>
		public void WasChanged(object source)
		{
			if (!Enabled)
			{
				return;
			}

			ReactiveManager.GetNode(source).NotifyChildren(notifyList);
			foreach (IListener toNotify in notifyList)
			{
				toNotify.Notify();
			}
			notifyList.Clear();
			(source as IListener)?.Notify();
		}

		/// <summary>
		/// Indicate that the given variable was read. May cause other objects to become dependend on the given variable.
		/// </summary>
		public void WasRead(object source)
		{
			if (!Enabled)
			{
				return;
			}

			if (DependentsEvaluating.Count != 0)
			{
				var weakDependent = DependentsEvaluating.Peek();
				ReactiveManager.GetNode(source).Add(weakDependent);
			}
		}

		/// <summary>
		/// Evaluation of reactive nodes must pass through this method.
		/// </summary>
		public T Evaluate<T>(IListener dependent, Func<T> func)
		{
			if (!Enabled)
			{
				return func();
			}

			WasRead(dependent);

			var node = ReactiveManager.GetNode(dependent);
			var dependentReference = node.GetDependency(dependent);
			DependentsEvaluating.Push(dependentReference);
			var result = func();
			DependentsEvaluating.Pop();
			return result;
		}
	}
}