using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SmartReactives.Common;

namespace SmartReactives.Test
{
    public class ReactiveSetTest
    {
        [Test]
        public void TestCount()
        {
            var reactiveList = new HashSet<int>().ToReactive();
            var countExpression = Reactive.Expression(() => reactiveList.Count);
            var counter = 0;
            countExpression.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
            var expectation = 1;
            reactiveList.Add(1);
            Assert.AreEqual(++expectation, counter);
            reactiveList.Add(2);
            Assert.AreEqual(++expectation, counter);
            Assert.AreEqual(expectation, counter);
            reactiveList.Remove(1);
            Assert.AreEqual(++expectation, counter);
            reactiveList.Clear();
            Assert.AreEqual(++expectation, counter);
        }

        [Test]
        public void TestContains()
        {
            var set = new HashSet<int> {1, 2, 3, 4}.ToReactive();
            var containsTwo = Reactive.Expression(() => set.Contains(2));
            var counter = 0;
            containsTwo.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
            var expectation = 1;
            set.Remove(2);
            Assert.AreEqual(++expectation, counter);
            set.Add(2);
            Assert.AreEqual(++expectation, counter);
            set.Clear();
            Assert.AreEqual(++expectation, counter);
        }

        [Test]
        public void TestEnumerator()
        {
            var reactiveList = new HashSet<int> { 1, 2, 3, 4 }.ToReactive();
            var sumFirstTwo = Reactive.Expression(() => reactiveList.Take(2).Sum());
            var counter = 0;
            sumFirstTwo.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
            var expectation = 1;
            reactiveList.Remove(2);
            Assert.AreEqual(++expectation, counter);
            reactiveList.Add(6);
            Assert.AreEqual(++expectation, counter);
            reactiveList.Clear();
            Assert.AreEqual(++expectation, counter);
        }

        [Test]
        public void TestEnumeratorEdge()
        {
            var reactiveList = new HashSet<int> { 1, 2, 3, 4 }.ToReactive();
            var outsideEnumerable = reactiveList.Take(2);
            var sumFirstTwo = Reactive.Expression(() => outsideEnumerable.Sum());
            var counter = 0;
            sumFirstTwo.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
            var expectation = 1;
            reactiveList.Remove(2);
            Assert.AreEqual(++expectation, counter);
            reactiveList.Add(6);
            Assert.AreEqual(++expectation, counter);
            reactiveList.Clear();
            Assert.AreEqual(++expectation, counter);
        }
    }
}