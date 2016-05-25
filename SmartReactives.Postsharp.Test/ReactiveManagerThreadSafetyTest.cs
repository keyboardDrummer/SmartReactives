using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SmartReactives.Core;
using SmartReactives.Extensions;
using SmartReactives.Test;

namespace SmartReactives.Postsharp.Test
{
	public class ReactiveManagerThreadSafetyTest
	{
		[Ignore("test is not entirely stable.")]
		[Test]
		public void HashSetNotThreadSafe()
		{
			var set = new HashSet<object>();

			var first = new Thread(() =>
			{
				for (var i = 0; i < 10000; i++)
				{
					set.Add(new object());
				}
			});
			var second = new Thread(() =>
			{
				for (var i = 0; i < 10000; i++)
				{
					set.Add(new object());
				}
			});

			first.Start();
			second.Start();
			first.Join();
			second.Join();

			Assert.True(20000 > set.Count);
		}

		[Test]
		public void ReactiveManagerForwardThreadSafety()
		{
			var dependency = new object();

			var firsts = Enumerable.Range(0, 10000).Select(_ => new Dependent()).ToList();
			var first = new Thread(() =>
			{
				foreach (var obj in firsts)
				{
					ReactiveManager.Evaluate(obj, () =>
					{
						ReactiveManager.WasRead(dependency);
						return true;
					});
				}
			});
			var seconds = Enumerable.Range(0, 10000).Select(_ => new Dependent()).ToList();
			var second = new Thread(() =>
			{
				foreach (var obj in seconds)
				{
					ReactiveManager.Evaluate(obj, () =>
					{
						ReactiveManager.WasRead(dependency);
						return true;
					});
				}
			});

			first.Start();
			second.Start();
			first.Join();
			second.Join();

			var dependencyCount = ReactiveManager.GetDependents(dependency).Count();
			Assert.AreEqual(20000, dependencyCount);
		}

		/// <summary>
		/// Makes a sink and 2 sources. The sink is only dependent on actualSource. However during evaluation of sink, we make sure another thread evaluates fakeSource.
		/// We assert that this does not cause the sink to become connected to fakeSource.
		/// </summary>
		[Test]
		public void MultiThreading()
		{
			var fakeSource = new Source();
			var actualSource = new Source();
			var waitEvaluateSource1 = new Waiter();
			var waitEvaluateSource2 = new Waiter();
			ReactiveExpression<bool> sink = null;
			var thread1 = new Thread(() =>
			{
				sink = new ReactiveExpression<bool>(() =>
				{
					waitEvaluateSource1.Release();
					waitEvaluateSource2.Wait();
					return actualSource.Woop;
				});
				sink.Evaluate();
			});
			var thread2 = new Thread(() =>
			{
				waitEvaluateSource1.Wait();
				Assert.AreEqual(fakeSource.Woop, fakeSource.Woop);
				waitEvaluateSource2.Release();
			});

			thread1.Start();
			thread2.Start();
			thread1.Join();
			thread2.Join();

			var counter = 0;
			sink.Subscribe(_ => counter++);
			Assert.AreEqual(1, counter);
			fakeSource.FlipWoop();
			Assert.AreEqual(1, counter);

			actualSource.FlipWoop();
			Assert.AreEqual(2, counter);
		}
	}
}
