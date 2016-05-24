//using System;
//using NUnit.Framework;
//using SmartReactives.Extensions;

//namespace SmartReactives.Examples
//{

//    class Tests
//    {
//        [Test]
//        public void Square()
//        {
//            var input = new ReactiveVariable<int>(1);
//            var inputSquared = new ReactiveExpression<int>(() => input.Value * input.Value);
//            inputSquared.Subscribe(getSquare => Console.WriteLine("square = " + getSquare()));

//            input.Value = 2;
//            input.Value = 3;
//        }

//        [Test]
//        public void Cache()
//        {
//            var input = new ReactiveVariable<int>(2); //We define a reactive variable.
//            Func<int> f = () => //f is the calculation we want to cache.
//            {
//                Console.WriteLine("f was evaluated");
//                return input.Value * input.Value; //f depends on our reactive variable input.
//            };
//            var cache = new ReactiveCache<int>(f); //We base our cache on f.

//            Console.WriteLine("f() = " + cache.Get()); //Cache was not set so we evaluate f.
//            Console.WriteLine("f() = " + cache.Get()); //Cache is set so we don't evaluate f.

//            input.Value = 3; //We change our input variable, causing our cache to become stale.
//            Console.WriteLine("f() = " + cache.Get()); //Cache is stale, so we must evaluate f.
//        }
//    }
//}