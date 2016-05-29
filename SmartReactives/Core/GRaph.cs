using System;
using System.Collections.Generic;

namespace SmartReactives.Core
{
	/// <summary>
	/// Contains methods related to Graphs.
	/// </summary>
	public static class Graph
	{
		/// <summary>
		/// Returns all nodes that are reachable from a set of root nodes.
		/// Also returns the roots themselves!
		/// </summary>
		public static IEnumerable<T> GetReachableNodes<T>(IEnumerable<T> roots, Func<T, IEnumerable<T>> getChildren)
		{
			var visited = new HashSet<T>();
			var nodesToVisit = new Stack<T>();
			foreach (var root in roots)
			{
				nodesToVisit.Push(root);
			}
			while (nodesToVisit.Count > 0)
			{
				var node = nodesToVisit.Pop();

				if (visited.Add(node))
				{
					foreach (var child in getChildren(node))
					{
						nodesToVisit.Push(child);
					}
				}
			}
			return visited;
		}
	}
}
