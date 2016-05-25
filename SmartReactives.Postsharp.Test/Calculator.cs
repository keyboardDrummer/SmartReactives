using System;
using NUnit.Framework;
using SmartReactives.PostSharp.NotifyPropertyChanged;

namespace SmartReactives.Postsharp.Test
{
    class Calculator : HasNotifyPropertyChanged
    {
        [SmartNotifyPropertyChanged]
        public int Number { get; set; }

        [SmartNotifyPropertyChanged]
        public int SquareOfNumber => Number * Number;

        [Test]
        public static void SquareDependsOnNumberTest()
        {
            var calculator = new Calculator();
            int counter = 0;

            Console.WriteLine("square = " + calculator.SquareOfNumber);
            calculator.PropertyChanged += (sender, eventArgs) =>
            {
                if (eventArgs.PropertyName == nameof(SquareOfNumber))
                {
                    counter += calculator.SquareOfNumber;
                }
            };

            Assert.AreEqual(0, counter);
            calculator.Number = 3;
            Assert.AreEqual(9, counter);
            calculator.Number = 4;
            Assert.AreEqual(25, counter);
            calculator.Number = 5;
            Assert.AreEqual(50, counter);
        }
    }
}