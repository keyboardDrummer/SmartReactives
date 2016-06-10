using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmartReactives.Common;

namespace SmartReactives.Test
{
    class DefaultListTest
    {
        class TestList<T> : DefaultList<T>
        {
            readonly IList<T> data;

            public TestList()
            {
                data = new List<T>();
            }

            public override IEnumerator<T> GetEnumerator()
            {
                return data.GetEnumerator();
            }

            public override void Insert(int index, T item)
            {
                data.Insert(index, item);
            }

            public override void RemoveAt(int index)
            {
                data.RemoveAt(index);
            }

            public override T this[int index]
            {
                get { return data[index]; }
                set { data[index] = value; }
            }

            public override void Clear()
            {
                data.Clear();
            }

            public override int Count => data.Count;
        }

        [Test]
        public void TestRemoveAndContains()
        {
            var list = new TestList<int>
            {
                1, 2, 3
            };
            Assert.False(list.Remove(4));
            Assert.True(list.Contains(2));
            Assert.True(list.Remove(2));
            Assert.False(list.Contains(2));
            list.Add(2);
            Assert.AreEqual(2, list.Last());
        }
    }
}
