using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SmartReactives.Common;
using SmartReactives.Core;

namespace SmartReactives.Test
{
    public class ReactiveManagerWeakStrongThreadSafetyTest
    {
        [Test]
        public void ReactiveManagerForwardThreadSafety()
        {
            var weak = new object();
            var weak2 = new object();
            var dependency = new WeakStrongReactive(weak, 0);

            var firsts = Enumerable.Range(0, 10000).Select(i => new WeakStrongReactive(weak2, i)).ToList();
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
            var weak3 = new object();
            var seconds = Enumerable.Range(0, 10000).Select(i => new WeakStrongReactive(weak3, i)).ToList();
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
    }
}