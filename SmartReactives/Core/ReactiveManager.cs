using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SmartReactives.Core
{
	/// <summary>
	/// The core of the reactive framework. Records dependencies between reactive nodes. Notifies nodes whenever their value changes.
	/// The ReactiveManager only works when used correctly. 
	/// Nodes must be evaluated through the ReactiveManager.Evaluate methode.
	/// The ReactiveManager must be notified whenever a node's value is changed by something external to the reactive framework, for example:
	///		- a property's underlying field is changed.
	///		- a property that depends on IO changes its value because of an IO change.
	/// 
	/// The ReactiveManager is thread-safe.
	/// The ReactiveManager is also weak. It will never cause a memory leak.
	/// </summary>
	public static class ReactiveManager
	{
	    static readonly ConditionalWeakTable<object, ReactiveNode> forward = new ConditionalWeakTable<object, ReactiveNode>();
	    static readonly ThreadLocal<ReactiveManagerThreadState> threadState = new ThreadLocal<ReactiveManagerThreadState>(() => new ReactiveManagerThreadState());

		/// <summary>
		/// Must be called whenever a node value is changed externally, for example when a property node's backing field changes its value.
		/// </summary>
		public static void WasChanged(object source)
		{
			threadState.Value.WasChanged(source);
		}

		/// <summary>
		/// Indicate that the given variable was read. May cause other objects to become dependend on the given variable.
		/// </summary>
		public static void WasRead(object source)
		{
			threadState.Value.WasRead(source);
		}

		/// <summary>
		/// Evaluation of reactive nodes must pass through this method.
		/// </summary>
		public static T Evaluate<T>(IListener dependent, Func<T> func)
		{
			return threadState.Value.Evaluate(dependent, func);
		}

	    internal static ReactiveManagerThreadState State => threadState.Value;

		/// <summary>
		/// Gets the dependents on the given node. Use for debugging.
		/// </summary>
		public static IEnumerable<IListener> GetDependents(object source)
		{
			Func<object, IEnumerable<IListener>> getChildren = 
				node => forward.GetOrCreateValue(node).GetCopy().Select(reference => reference.Value).Where(child => child != null);
			return Graph.GetReachableNodes<IListener>(getChildren(source), getChildren);
		}

		internal static ReactiveNode GetNode(object key)
		{
			return forward.GetValue(key, CreateList);
		}

		static ReactiveNode CreateList(object key)
		{
			return new ReactiveNode();
		}
	}
}
