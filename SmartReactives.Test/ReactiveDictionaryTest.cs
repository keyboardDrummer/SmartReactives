using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmartReactives.Common;

namespace SmartReactives.Test
{
    class ReactiveDictionaryTest
    {
        [Test]
        public void TestCount()
        {
            var reactiveDictionary = new Dictionary<int, string>().ToReactive();
            var countExpression = Reactive.Expression(() => reactiveDictionary.Count);
            var counter = 0;
            countExpression.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
            var expectation = 1;
            reactiveDictionary.Add(1, "first");
            Assert.AreEqual(++expectation, counter);
            reactiveDictionary.Add(2, "second");
            Assert.AreEqual(++expectation, counter);
            reactiveDictionary[0] = "third";
            Assert.AreEqual(expectation, counter);
            reactiveDictionary.Remove(2);
            Assert.AreEqual(++expectation, counter);
            reactiveDictionary.Clear();
            Assert.AreEqual(++expectation, counter);
        }

        [Test]
        public void TestKey()
        {
            var reactiveList = new Dictionary<int, string> { { 1, "first"}, {2, "second"}, { 3, "third" }, { 4, "fourth" } }.ToReactive();
            var countExpression = Reactive.Expression(() => reactiveList[2]);
            var counter = 0;
            countExpression.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
            var expectation = 1;
            reactiveList[2] = "fifth";
            Assert.AreEqual(++expectation, counter);
            reactiveList[1] = "sixth";
            Assert.AreEqual(expectation, counter);
            reactiveList[3] = "seventh";
            Assert.AreEqual(expectation, counter);
            reactiveList.Add(8, "eight");
            Assert.AreEqual(expectation, counter);
            reactiveList.Remove(8);
            Assert.AreEqual(expectation, counter);
            reactiveList.Remove(1);
            Assert.AreEqual(expectation, counter);
        }

        [Test]
        public void TestEnumerator()
        {
            var reactiveList = new Dictionary<int, string> { { 1, "first" }, { 2, "second" }, { 3, "third" }, { 4, "fourth" } }.ToReactive();
            var sumFirstTwo = Reactive.Expression(() => reactiveList.Take(2).Sum(kv => kv.Key));
            var counter = 0;
            sumFirstTwo.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
            var expectation = 1;
            reactiveList[3] = "fifth";
            Assert.AreEqual(expectation, counter);
            reactiveList.Add(6, "sixth");
            Assert.AreEqual(expectation, counter);
            reactiveList[1] = "seventh";
            Assert.AreEqual(++expectation, counter);
            reactiveList.Clear();
            Assert.AreEqual(++expectation, counter);
        }
    }
}
