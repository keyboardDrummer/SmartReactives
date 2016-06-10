using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmartReactives.Common;

namespace SmartReactives.Test
{
    class TestDefaultDictionary
    {
        class TestDictionary<Key,Value> : DefaultDictionary<Key, Value>
        {
            readonly IDictionary<Key, Value> data = new Dictionary<Key, Value>();
            public override bool ContainsKey(Key key)
            {
                return data.ContainsKey(key);
            }

            public override void Add(Key key, Value value)
            {
                data.Add(key, value);
            }

            public override bool Remove(Key key)
            {
                return data.Remove(key);
            }

            public override bool TryGetValue(Key key, out Value value)
            {
                return data.TryGetValue(key, out value);
            }

            public override Value this[Key key]
            {
                get { return data[key]; }
                set { data[key] = value; }
            }

            public override IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
            {
                return data.GetEnumerator();
            }

            public override void Clear()
            {
                data.Clear();
            }

            public override int Count => data.Count;
        }

        [Test]
        public void TestContainsRemoveAdd()
        {
            var dictionary = new TestDictionary<int, string>();
            dictionary.Add(new KeyValuePair<int, string>(0, "0"));
            Assert.AreEqual(1, dictionary.Count);
            Assert.True(dictionary.Contains(new KeyValuePair<int, string>(0, "0")));
            Assert.True(dictionary.Remove(new KeyValuePair<int, string>(0, "0")));
            Assert.False(dictionary.Contains(new KeyValuePair<int, string>(0, "0")));
            Assert.AreEqual(0, dictionary.Count);
        }
    }
}
