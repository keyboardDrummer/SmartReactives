using System;
using NUnit.Framework;
using SmartReactives.PostSharp;

namespace SmartReactives.PostsharpExamples
{
	class CachingCalculator
	{
		[ReactiveVariable]
		public int Input { get; set; }

		[ReactiveCache]
		public double Square
		{
			get
			{
				Console.WriteLine("cache miss");
				return Math.Pow(Input, 2);
			}
		}

		[Test]
		public static void ReactiveCache()
		{
			var calculator = new CachingCalculator();
			calculator.Input = 2;

			Assert.AreEqual(4, calculator.Square); //Cache miss. Prints 'cache miss'
			Assert.AreEqual(4, calculator.Square); //Cache hit.

			calculator.Input = 3; //Cache becomes stale.

			Assert.AreEqual(9, calculator.Square); // Cache miss. Prints 'cache miss'
			Assert.AreEqual(9, calculator.Square); // Cache hit.
		}
	}
}