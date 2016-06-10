using System.Collections.Generic;
using NUnit.Framework;
using SmartReactives.Common;

namespace SmartReactives.Test
{
    class TestDefaultCollection
    {
        class TestCollection<T> : DefaultCollection<T>
        {
            readonly ICollection<T> data = new List<T>();
            public override IEnumerator<T> GetEnumerator()
            {
                return data.GetEnumerator();
            }

            public void Add(T item)
            {
                data.Add(item);
            }

            public override void Clear()
            {
                data.Clear();;
            }

            public override bool Contains(T item)
            {
                return data.Contains(item);
            }

            public override bool Remove(T item)
            {
                return data.Remove(item);
            }

            public override int Count => data.Count;
        }

        [Test]
        public void Test()
        {
            var collection = new TestCollection<int>();
            collection.Add(1);
            collection.Add(2);
            collection.Add(3);
            int[] target = new int[5];
            collection.CopyTo(target,1);
            Assert.AreEqual(0, target[0]);
            Assert.AreEqual(1, target[1]);
            Assert.AreEqual(3, target[3]);
            Assert.AreEqual(0, target[4]);
        }
    }
}
