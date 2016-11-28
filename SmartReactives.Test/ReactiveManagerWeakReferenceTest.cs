using System;
using System.Linq;
using NUnit.Framework;
using SmartReactives.Common;
using SmartReactives.Core;

namespace SmartReactives.Test
{
    public class ReactiveManagerWeakReferenceTest
    {
        [Test]
        public void TestWeakness()
        {
            var source = new Source();
            var weakReference = CreateWeakReactive(source);
            Assert.AreEqual(true, weakReference.IsAlive); //Fragile. GC might have run.
            GC.Collect();
            Assert.AreEqual(false, weakReference.IsAlive);
        }

        static WeakReference CreateWeakReactive(Source source)
        {
            var weakReactiveVariable = new ReactiveExpression<bool>(() => source.Woop);
            weakReactiveVariable.Evaluate();
            var counter = 0;
            weakReactiveVariable.Subscribe(_ => counter++);
            Assert.AreEqual(1, counter);
            source.Woop = !source.Woop;
            Assert.AreEqual(2, counter);
            var result = new WeakReference(weakReactiveVariable);
            Assert.AreEqual(true, result.IsAlive);
            return result;
        }

        [Test]
        public void TestDependentWeakness()
        {
            var dependency = new object();

            foreach (var obj in Enumerable.Range(0, 10000))
            {
                ReactiveManager.Evaluate(new Dependent("TestDependentWeakness"), () =>
                {
                    ReactiveManager.WasRead(dependency);
                    return true;
                });
            }

            GC.Collect();
            Assert.AreEqual(0, ReactiveManager.GetDependents(dependency).Count());
        }

		[Test]
		public void TestReactiveExpressionWeakness()
		{
			var dependency = new ReactiveVariable<int>();

			AddDependents(dependency, false);

			Assert.AreEqual(10000, ReactiveManager.GetDependents(dependency).Count());
			GC.Collect();
			Assert.AreEqual(10000, ReactiveManager.GetDependents(dependency).Count());
		}

		[Test]
		public void TestReactiveExpressionWeakness2()
		{
			var dependency = new ReactiveVariable<int>();

			AddDependents(dependency, true);

			GC.Collect();
			Assert.AreEqual(0, ReactiveManager.GetDependents(dependency).Count());
		}

		static void AddDependents(ReactiveVariable<int> dependency, bool weak)
	    {
		    int sum = 0;
		    foreach (var obj in Enumerable.Range(0, 10000))
		    {
			    var expression = new ReactiveExpression<int>(() => dependency.Value, weak);
			    sum += expression.Evaluate();
		    }
	    }
    }
}