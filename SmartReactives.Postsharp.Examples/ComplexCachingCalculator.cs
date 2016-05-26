using System;
using NUnit.Framework;
using SmartReactives.PostSharp;

namespace SmartReactives.PostsharpExamples
{
	class ComplexCachingCalculator
	{
		[ReactiveVariable]
		public bool UseSquareInsteadOfMinusOne { get; set; }

		[ReactiveVariable]
		public int SquareInput { get; set; }

		public int CacheMisses { get; private set; }

		[ReactiveCache]
		public double SquareOrReturnMinusOne
		{
			get
			{
				CacheMisses++;
				return UseSquareInsteadOfMinusOne ? Math.Pow(SquareInput, 2) : -1;
			}
		}

		[Test]
		public static void ReactiveCache()
		{
			var calculator = new ComplexCachingCalculator();
			calculator.UseSquareInsteadOfMinusOne = false;
			Assert.AreEqual(-1, calculator.SquareOrReturnMinusOne); //Cache miss
			Assert.AreEqual(1, calculator.CacheMisses);
			Assert.AreEqual(-1, calculator.SquareOrReturnMinusOne); //Cache hit
			Assert.AreEqual(1, calculator.CacheMisses); 
			calculator.SquareInput = 2;  //Cache remains up=to-date, since it does it depend on SquareInput yet.
			Assert.AreEqual(-1, calculator.SquareOrReturnMinusOne); //Cache hit
			Assert.AreEqual(1, calculator.CacheMisses);
			calculator.UseSquareInsteadOfMinusOne = true; //Cache becomes stale, since depends on UseSquareInsteadOfMinusOne.
			Assert.AreEqual(4, calculator.SquareOrReturnMinusOne); //Cache miss
			Assert.AreEqual(2, calculator.CacheMisses);
			calculator.SquareInput = 3; //Now the cache does depend on SquareInput, so it becomes stale.
			Assert.AreEqual(9, calculator.SquareOrReturnMinusOne); //Cache miss
			Assert.AreEqual(3, calculator.CacheMisses);
		}
	}
}