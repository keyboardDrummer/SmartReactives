using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using SmartReactives.Extensions;
using SmartReactives.List;

namespace SmartReactives.Test
{
    class ReactiveManagerWithListTest
    {
        [Test]
        public void NotifyCollectionChangedAsObservable()
        {
            var collection = new ObservableCollection<int>();
            var observable = new NotifyCollectionChangedAsObservable(collection);
            var counter = 0;
            var expectation = 0;
            using (observable.Subscribe(_ => counter++))
            {
                collection.Add(3);
                Assert.AreEqual(++expectation, counter);
                collection.Clear();
                Assert.AreEqual(++expectation, counter);
            }

            collection.Add(3);
            Assert.AreEqual(expectation, counter);
        }

        [Test]
        public void TestBasics()
        {
            var collection = new ObservableCollection<int>();
            collection.Add(1);
            collection.Add(2);
            collection.Add(3);
            Func<ObservableCollection<int>> getCollection = () => ReactiveManagerWithList.Evaluate(() => collection);
            var secondElement = new ReactiveExpression<int>(() => getCollection()[1]);
            var counter = 0;
            var expectation = 0;
            secondElement.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
            Assert.AreEqual(++expectation, counter);
            collection.RemoveAt(2);
            Assert.AreEqual(++expectation, counter);
            collection.Add(0);
            Assert.AreEqual(++expectation, counter);
        }
    }
}
