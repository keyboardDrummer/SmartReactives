using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using SmartReactives.Common;

namespace SmartReactives.Examples
{
    class Examples
    {
        public void Square()
        {
            var input = new ReactiveVariable<int>(1);
            var inputSquared = new ReactiveExpression<int>(() => input.Value * input.Value);
            inputSquared.Subscribe(getSquare => Console.WriteLine("square = " + getSquare())); // Prints 'square = 1'

            input.Value = 2; // Prints 'square = 4'
            input.Value = 3; // Prints 'square = 9'
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
            var input = new ReactiveVariable<int>();
            var timesTwo = new ReactiveExpression<int>(() => input.Value * 2);
            var timesThree = new ReactiveExpression<int>(() => input.Value * 3);
            var sumOfBoth = new ReactiveExpression<int>(() => timesTwo.Evaluate() + timesThree.Evaluate());
            sumOfBoth.Subscribe(getValue => Console.WriteLine("sumOfBoth = " + getValue())); //Prints 'sumOfBoth = 0'
            input.Value = 1; //Prints 'sumOfBoth = 5'
            input.Value = 2; //Prints 'sumOfBoth = 10'
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
            var left = new ReactiveVariable<bool>();
            var right = new ReactiveVariable<bool>();
            var leftOrRight = new ReactiveExpression<bool>(() => left.Value || right.Value);
            leftOrRight.Subscribe(getValue => Console.WriteLine("leftOrRight = " + getValue())); // Prints 'leftOrRight = False'

            right.Value = true; // Prints 'leftOrRight = True'
            left.Value = true; // Prints 'leftOrRight = True'
            right.Value = false; // Prints nothing
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

        public void Cache()
        {
            var input = new ReactiveVariable<int>(2); //We define a reactive variable.
            Func<int> f = () => //f is the calculation we want to cache.
            {
                Console.WriteLine("f was evaluated");
                return input.Value * input.Value; //f depends on our reactive variable input.
            };
            var cache = new ReactiveCache<int>(f); //We base our cache on f.

            Console.WriteLine("f() = " + cache.Get()); //Cache was not set so we evaluate f.
            Console.WriteLine("f() = " + cache.Get()); //Cache is set so we don't evaluate f.

            input.Value = 3; //We change our input variable, causing our cache to become stale.
            Console.WriteLine("f() = " + cache.Get()); //Cache is stale, so we must evaluate f.
        }
    }
}