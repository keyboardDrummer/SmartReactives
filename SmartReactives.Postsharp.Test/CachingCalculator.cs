using System;
using NUnit.Framework;
using SmartReactives.PostSharp;

namespace SmartReactives.Postsharp.Test
{
    class CachingCalculator
    {
        [ReactiveVariable]
        public bool UseInput { get; set; }

        [ReactiveVariable]
        public int ComplexInput { get; set; }

        public int CacheMisses { get; private set; }

        [ReactiveCache]
        public double HeavyCalculation
        {
            get
            {
                CacheMisses++;
                return UseInput ? Math.Pow(ComplexInput, 3) : -1;
            }
        }

        [Test]
        public static void ReactiveCache()
        {
            var calculator = new CachingCalculator();
            calculator.UseInput = false;
            Assert.AreEqual(-1, calculator.HeavyCalculation);
            Assert.AreEqual(1, calculator.CacheMisses);
            Assert.AreEqual(-1, calculator.HeavyCalculation);
            Assert.AreEqual(1, calculator.CacheMisses);
            calculator.ComplexInput = 2; //HeavyCalculation does not depend on ComplexInput yet, so our cache is still up-to-date.
            Assert.AreEqual(-1, calculator.HeavyCalculation);
            Assert.AreEqual(1, calculator.CacheMisses);
            calculator.UseInput = true;
            Assert.AreEqual(8, calculator.HeavyCalculation);
            Assert.AreEqual(2, calculator.CacheMisses);
            calculator.ComplexInput = 3; //Now HeavyCalculation does depend on ComplexInput, so the cache becomes stale.
            Assert.AreEqual(27, calculator.HeavyCalculation);
            Assert.AreEqual(3, calculator.CacheMisses);
        }
    }
}