using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NUnit.Framework;
using SmartReactives.Common;

namespace SmartReactives.Examples
{
    class Examples
    {
        public void Square()
        {
            var input = Reactive.Variable(1);
            var inputSquared = Reactive.Expression(() => input * input);
            inputSquared.Subscribe(getSquare => Console.WriteLine("square = " + getSquare())); // Prints 'square = 1'

            input.Value = 2; //Prints 'square = 4'
            input.Value = 3; //Prints 'square = 9'
        }

        public void RxSquare()
        {
            var inputs = new BehaviorSubject<int>(1);
            var inputSquared = inputs.Select(input => input * input);
            inputSquared.Subscribe(square => Console.WriteLine("square = " + square)); // Prints 'square = 1'

            inputs.OnNext(2); // Prints 'square = 4'
            inputs.OnNext(3); // Prints 'square = 9'
        }

        public void Composition()
        {
            var input = Reactive.Variable(1);
            var timesTwo = Reactive.Expression(() => input * 2);
            var timesThree = Reactive.Expression(() => input * 3);
            var sumOfBoth = Reactive.Expression(() => timesTwo.Evaluate() + timesThree.Evaluate());
            sumOfBoth.Subscribe(getValue => Console.WriteLine("sumOfBoth = " + getValue())); //Prints 'sumOfBoth = 5'
            input.Value = 2; //Prints 'sumOfBoth = 10'
            input.Value = 3; //Prints 'sumOfBoth = 15'
        }

        public void RxComposition()
        {
            var input = new BehaviorSubject<int>(0);
            var timesTwo = input.Select(value => value * 2);
            var timesThree = input.Select(value => value * 3);
            var sumOfBoth = timesTwo.CombineLatest(timesThree, (first, second) => first + second);
            sumOfBoth.Subscribe(value => Console.WriteLine("sumOfBoth = " + value)); //Prints 'sumOfBoth = 0'
            input.OnNext(1); //Prints 'sumOfBoth = 2' and 'sumOfBoth = 5'
            input.OnNext(2); //Prints 'sumOfBoth = 7' and 'sumOfBoth = 10'
        }

        public void Precise()
        {
            var left = Reactive.Variable(false);
            var right = Reactive.Variable(false);
            var leftOrRight = Reactive.Expression(() => left || right);
            leftOrRight.Subscribe(getValue => Console.WriteLine("leftOrRight = " + getValue()));
            //Prints 'leftOrRight = False'

            right.Value = true; //Prints 'leftOrRight = True'
            left.Value = true; //Prints 'leftOrRight = True'
            right.Value = false; //Prints nothing
        }

        public void RxPrecise()
        {
            var lefts = new BehaviorSubject<bool>(false);
            var rights = new BehaviorSubject<bool>(false);
            var leftOrRight = lefts.CombineLatest(rights, (left, right) => left || right);

            leftOrRight.Subscribe(value => Console.WriteLine("leftOrRight = " + value)); // Prints 'leftOrRight = False'

            rights.OnNext(true); // Prints 'leftOrRight = True'
            lefts.OnNext(true); // Prints 'leftOrRight = True'
            rights.OnNext(false); // Prints 'leftOrRight = True' (while SmartReactives prints nothing here)
        }

        [Test]
        public void Cache()
        {
            var input = Reactive.Variable(2); //We define a reactive variable.
            Func<int> f = () => //f is the calculation we want to cache.
            {
                Console.WriteLine("cache miss");
                return input * input; //f depends on our reactive variable 'input'.
            };
            var cache = Reactive.Cache(f); //We base our cache on f.

            Assert.AreEqual(4, cache.Get()); //Prints 'cache miss'
            Assert.AreEqual(4, cache.Get()); //Cache hit.

            input.Value = 3; //Cache becomes stale.

            Assert.AreEqual(9, cache.Get()); //Prints 'cache miss'
            Assert.AreEqual(9, cache.Get()); //Cache hit.
        }
    }
}