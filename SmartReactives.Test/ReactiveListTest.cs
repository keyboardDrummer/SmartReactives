using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SmartReactives.Common;

namespace SmartReactives.Test
{
    public class ReactiveListTest
    {
        [Test]
        public void TestCount()
        {
            var reactiveList = new List<int>().ToReactive();
            var countExpression = Reactive.Expression(() => reactiveList.Count);
            var counter = 0;
            countExpression.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
            var expectation = 1;
            reactiveList.Add(1);
            Assert.AreEqual(++expectation, counter);
            reactiveList.Add(2);
            Assert.AreEqual(++expectation, counter);
            reactiveList[0] = 3;
            Assert.AreEqual(expectation, counter);
            reactiveList.RemoveAt(0);
            Assert.AreEqual(++expectation, counter);
            reactiveList.Clear();
            Assert.AreEqual(++expectation, counter);
        }

        [Test]
        public void TestIndex()
        {
            var reactiveList = new List<int>() {1, 2, 3, 4}.ToReactive();
            var countExpression = Reactive.Expression(() => reactiveList[2]);
            var counter = 0;
            countExpression.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
            var expectation = 1;
            reactiveList[2] = 5;
            Assert.AreEqual(++expectation, counter);
            reactiveList[1] = 6;
            Assert.AreEqual(expectation, counter);
            reactiveList[3] = 7;
            Assert.AreEqual(expectation, counter);
            reactiveList.Add(8);
            Assert.AreEqual(expectation, counter);
            reactiveList.RemoveAt(reactiveList.Count - 1);
            Assert.AreEqual(expectation, counter);
            reactiveList.RemoveAt(0);
            Assert.AreEqual(++expectation, counter);
        }

        [Test]
        public void TestEnumerator()
        {
            var reactiveList = new List<int>() { 1, 2, 3, 4 }.ToReactive();
            var sumFirstTwo = Reactive.Expression(() => reactiveList.Take(2).Sum());
            var counter = 0;
            sumFirstTwo.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
            var expectation = 1;
            reactiveList[2] = 5;
            Assert.AreEqual(expectation, counter);
            reactiveList.Add(6);
            Assert.AreEqual(expectation, counter);
            reactiveList[1] = 7;
            Assert.AreEqual(++expectation, counter);
            reactiveList.Clear();
            Assert.AreEqual(++expectation, counter);
        }
    }
}