using System;
using System.Linq;
using SmartReactives.Core;

namespace SmartReactives.Examples
{
	class Profiling
	{
		public static void Main(string[] args)
		{
			Console.Read();

			TestManyToOneDependency();
		}

		static void TestManyToOneDependency()
		{
			var dependency = new CompositeReactiveObject(new object(), 0);
			var dependent = new object();
			foreach (var i in Enumerable.Range(0, 100000))
			{
				ReactiveManager.Evaluate(new CompositeReactiveObject(dependent, i), () =>
				{
					ReactiveManager.WasRead(dependency);
					return true;
				});
			}
		}
	}
}