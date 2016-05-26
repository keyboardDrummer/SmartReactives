using System;
using NUnit.Framework;
using SmartReactives.PostSharp;

namespace SmartReactives.PostsharpExamples
{
	class CachingCalculator
	{
		[ReactiveVariable]
		public int Input { get; set; }

		public int CacheMisses { get; private set; }

		[ReactiveCache]
		public double Square
		{
			get
			{
				CacheMisses++;
				return Math.Pow(Input, 2);
			}
		}

		[Test]
		public static void ReactiveCache()
		{
			var calculator = new CachingCalculator();
			calculator.Input = 2;

			Assert.AreEqual(4, calculator.Square); //Cache miss
			Assert.AreEqual(1, calculator.CacheMisses);
			Assert.AreEqual(4, calculator.Square); //Cache hit
			Assert.AreEqual(1, calculator.CacheMisses); 

			calculator.Input = 3;  //Cache remains up=to-date, since it does it depend on SquareInput yet.

			Assert.AreEqual(9, calculator.Square); //Cache miss
			Assert.AreEqual(2, calculator.CacheMisses);
			Assert.AreEqual(9, calculator.Square); //Cache hit
			Assert.AreEqual(2, calculator.CacheMisses);
		}
	}
}