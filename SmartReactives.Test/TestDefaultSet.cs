using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SmartReactives.Common;

namespace SmartReactives.Test
{
    class TestDefaultSet
    {
        class TestSet<T> : DefaultSet<T>
        {
            readonly ISet<T> data = new HashSet<T>();
            public override IEnumerator<T> GetEnumerator()
            {
                return data.GetEnumerator();
            }

            public override bool Add(T item)
            {
                return data.Add(item);
            }

            public override void Clear()
            {
                data.Clear();
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
        public void TestUnionWith()
        {
            var set = new TestSet<int>
            {
                1, 2, 3, 4
            };
            set.UnionWith(new[] { 3,4,5});
            Assert.True(new[] {1,2,3,4,5}.SequenceEqual(set));
        }

        [Test]
        public void TestIntersectWith()
        {
            var set = new TestSet<int>
            {
                1, 2, 3, 4
            };
            set.IntersectWith(new[] { 3, 4, 5 });
            Assert.True(new[] { 3, 4 }.SequenceEqual(set));
        }

        [Test]
        public void TestExceptWith()
        {
            var set = new TestSet<int>
            {
                1, 2, 3, 4
            };
            set.ExceptWith(new[] { 3, 4, 5 });
            Assert.True(new[] { 1, 2 }.SequenceEqual(set));
        }
        
        [Test]
        public void TestIsSupersetOf()
        {
            var set = new TestSet<int>
            {
                1, 2, 3, 4
            };
            Assert.False(set.IsSupersetOf(new[] { 3, 4, 5 }));
            Assert.True(set.IsSupersetOf(new[] { 3, 4 }));
            Assert.True(set.IsSupersetOf(new[] { 1, 2, 3, 4 }));
        }

        [Test]
        public void TestIsProperSupersetOf()
        {
            var set = new TestSet<int>
            {
                1, 2, 3, 4
            };
            Assert.False(set.IsProperSupersetOf(new[] { 3, 4, 5 }));
            Assert.True(set.IsProperSupersetOf(new[] { 3, 4 }));
            Assert.False(set.IsProperSupersetOf(new[] { 1, 2, 3, 4 }));
        }


        [Test]
        public void TestIsSubsetOf()
        {
            var set = new TestSet<int>
            {
                1, 2, 3, 4
            };
            Assert.False(set.IsSubsetOf(new[] { 3, 4, 5 }));
            Assert.True(set.IsSubsetOf(new[] { 1,2, 3, 4,5  }));
            Assert.True(set.IsSubsetOf(new[] { 1, 2, 3, 4 }));
        }

        [Test]
        public void TestIsProperSubsetOf()
        {
            var set = new TestSet<int>
            {
                1, 2, 3, 4
            };
            Assert.False(set.IsProperSubsetOf(new[] { 3, 4, 5 }));
            Assert.True(set.IsProperSubsetOf(new[] { 1,2,3, 4,5 }));
            Assert.False(set.IsProperSubsetOf(new[] { 1, 2, 3, 4 }));
        }

        [Test]
        public void TestOverlaps()
        {
            var set = new TestSet<int>
            {
                1, 2, 3, 4
            };
            Assert.True(set.Overlaps(new[] { 3, 4, 5 }));
            Assert.False(set.Overlaps(new[] { 5,6 }));
            Assert.False(set.Overlaps(new int[] {  }));
            Assert.True(set.Overlaps(new int[] { 1,2,3,4 }));
        }

        [Test]
        public void TestSetEquals()
        {
            var set = new TestSet<int>
            {
                1, 2, 3, 4
            };
            Assert.False(set.SetEquals(new[] { 3, 4, 5 }));
            Assert.False(set.SetEquals(new[] { 5, 6 }));
            Assert.False(set.SetEquals(new int[] { }));
            Assert.True(set.SetEquals(new int[] { 1, 2, 3, 4 }));
        }
    }
}
