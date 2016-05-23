using System;
using NUnit.Framework;
using SmartReactives.Extensions;

namespace SmartReactives.Examples
{

    class Tests
    {
        [Test]
        public void SquareInput()
        {
            var input = new ReactiveVariable<int>(1);
            var inputSquared = new ReactiveExpression<int>(() => input.Value * input.Value);
            inputSquared.Subscribe(getSquare => Console.WriteLine("input squared = " + getSquare()));

            input.Value = 2;
            input.Value = 3;
        }

        //[Test]
        //public void TallEnoughDependsOnLength()
        //{
        //    var length = new ReactiveVariable<int>(120);
        //    var tallEnough = new ReactiveExpression<bool>(() => length.Value > 140);
        //    var tallEnoughChangedCounter = 0;
        //    tallEnough.Subscribe(_ => tallEnoughChangedCounter++);

        //    Assert.AreEqual(false, tallEnough.Evaluate());
        //    length.Value = 180;

        //    Assert.AreEqual(true, tallEnough.Evaluate());
        //    Assert.AreEqual(1, tallEnoughChangedCounter);
        //}

        [Test]
        public void Cache()
        {
            var input = new ReactiveVariable<int>(1); //We define a reactive variable.
            Func<int> f = () => //f is the calculation we want to cache.
            {
                Console.WriteLine("f was evaluated");
                return 3 * input.Value; //f depends on our reactive variable input.
            };
            var cache = new ReactiveCache<int>(f); //We base our cache on f.

            Console.WriteLine("f() = " + cache.Get()); //Cache was not set so we evaluate f.
            Console.WriteLine("f() = " + cache.Get()); //Cache is set so we don't evaluate f.

            input.Value = 2; //We change our input variable, causing our cache to become stale.
            Console.WriteLine("f() = " + cache.Get()); //Cache is stale, so we must evaluate f.
        }
    }
}