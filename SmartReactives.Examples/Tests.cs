using System;
using NUnit.Framework;
using SmartReactives.Extensions;

namespace SmartReactives.Examples
{
	class Tests
	{
	    [Test]
	    public void Nesting()
	    {
	        var input = new ReactiveVariable<int>();
            var timesTwo = new ReactiveExpression<int>(() => input.Value * 2);
            var timesThree = new ReactiveExpression<int>(() => input.Value * 3);
            var sumOfBoth = new ReactiveExpression<int>(() => timesTwo.Evaluate() + timesThree.Evaluate());
	        sumOfBoth.Subscribe(getValue => Console.WriteLine("sumOfBoth = " + getValue())); //Prints 'sumOfBoth = 0'
            input.Value = 1; //Prints 'sumOfBoth = 5'
            input.Value = 2; //Prints 'sumOfBoth = 10'
        }

		[Test]
		public void Precise()
		{
			var left = new ReactiveVariable<bool>();
			var right = new ReactiveVariable<bool>();
			var leftOrRight = new ReactiveExpression<bool>(() => left.Value || right.Value);
			leftOrRight.Subscribe(getValue => Console.WriteLine("leftOrRight = " + getValue())); // Prints 'leftOrRight = false'

			right.Value = true; // Prints 'leftOrRight = true'
			left.Value = true; // Prints 'leftOrRight = true'
			right.Value = false; // Prints nothing
		}

		[Test]
		public void Square()
		{
			var input = new ReactiveVariable<int>(1);
			var inputSquared = new ReactiveExpression<int>(() => input.Value * input.Value);
			inputSquared.Subscribe(getSquare => Console.WriteLine("square = " + getSquare()));

			input.Value = 2;
			input.Value = 3;
		}

		[Test]
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