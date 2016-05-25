using System;
using NUnit.Framework;
using SmartReactives.Extensions;
using SmartReactives.PostSharp.NotifyPropertyChanged;

namespace SmartReactives.PostsharpExamples
{
	class Calculator : HasNotifyPropertyChanged
    {
        [SmartNotifyPropertyChanged]
        public int Number { get; set; }

        [SmartNotifyPropertyChanged]
        public int SquareOfNumber => Number * Number;

        [Test]
        public static void SquareDependsOnNumber()
        {
            var calculator = new Calculator();
            calculator.Number = 2;
            
            Console.WriteLine("square = " + calculator.SquareOfNumber); 
            calculator.PropertyChanged += (sender, eventArgs) =>
            {
                if (eventArgs.PropertyName == nameof(SquareOfNumber))
                    Console.WriteLine("square = " + calculator.SquareOfNumber);
            };

            calculator.Number = 3;
            calculator.Number = 4;
            calculator.Number = 5;
        }
    }
}
