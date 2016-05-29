using System;
using NUnit.Framework;
using SmartReactives.Extensions;

namespace SmartReactives.Test
{
	public class ReactiveCacheTest
    {
	    [Test]
	    public void TestInvalidate()
        {
            var counter = 0;
            var source = new DebugReactiveVariable<int>(0, "source");
            Func<int> cacheFunc = () =>
            {
                counter++;
                return source.Value;
            };
            var cache = new ReactiveCache<int>(cacheFunc);
	        var expectation = 0;
            Assert.AreEqual(0, cache.Get());
            Assert.AreEqual(++expectation, counter);
            Assert.AreEqual(0, cache.Get());
            cache.Invalidate();
            Assert.AreEqual(++expectation, counter);
        }

	    [Test]
		public void CanCache()
		{
			var counter = 0;
			var source = new DebugReactiveVariable<int>(0, "source");
			Func<int> cacheFunc = () =>
			{
				counter++;
				return source.Value;
			};
			var cache = new ReactiveCache<int>(cacheFunc);
			Assert.AreEqual(0, counter);
			Assert.AreEqual(0, cache.Get());
			Assert.AreEqual(1, counter);
			Assert.AreEqual(0, cache.Get());
			Assert.AreEqual(1, counter);
		}

		[Test]
		public void CanInvalidate()
		{
			var counter = 0;
			var source = new DebugReactiveVariable<int>(0, "source");
			Func<int> cacheFunc = () =>
			{
				counter++;
				return source.Value;
			};
			var cache = new ReactiveCache<int>(cacheFunc);
			Assert.AreEqual(0, counter);
			Assert.AreEqual(0, cache.Get());
			Assert.AreEqual(1, counter);
			source.Value = 1;
			Assert.AreEqual(1, cache.Get());
			Assert.AreEqual(2, counter);
		}
	}
}
