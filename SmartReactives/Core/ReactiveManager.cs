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
	/// The ReactiveManager is also weak, it will never cause a memory leak.
	/// </summary>
	public static class ReactiveManager
	{
		private static readonly ConditionalWeakTable<object, ReactiveNode> _forward = new ConditionalWeakTable<object, ReactiveNode>();
		private static readonly ThreadLocal<ReactiveManagerThreadState> _threadState = new ThreadLocal<ReactiveManagerThreadState>(() => new ReactiveManagerThreadState());

		/// <summary>
		/// Must be called whenever a node value is changed externally, for example when a property node's backing field changes its value.
		/// </summary>
		public static void WasChanged(object source)
		{
			_threadState.Value.WasChanged(source);
		}

		/// <summary>
		/// Indicate that the given variable was read. May cause other objects to become dependend on the given variable.
		/// </summary>
		public static void WasRead(object source)
		{
			_threadState.Value.WasRead(source);
		}

		/// <summary>
		/// Evaluation of reactive nodes must pass through this method.
		/// </summary>
		public static T Evaluate<T>(IListener dependent, Func<T> func)
		{
			return _threadState.Value.Evaluate(dependent, func);
		}

		public static ReactiveManagerThreadState State => _threadState.Value;

		/// <summary>
		/// Gets the dependents on the given node. Use for debugging.
		/// </summary>
		public static IEnumerable<IListener> GetDependents(object source)
		{
			Func<object, IEnumerable<IListener>> getChildren = 
				node => _forward.GetOrCreateValue(node).GetCopy().Select(reference => reference.Value).Where(child => child != null);
			return Graph.GetReachableNodes<IListener>(getChildren(source), getChildren);
		}

		internal static ReactiveNode GetNode(object key)
		{
			return _forward.GetValue(key, CreateList);
		}

		static ReactiveNode CreateList(object key)
		{
			return new ReactiveNode();
		}
	}
}
