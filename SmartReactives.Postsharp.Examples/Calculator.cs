using System;
using NUnit.Framework;
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
            calculator.Number = 1;

            Console.WriteLine("square = " + calculator.SquareOfNumber); //Prints 'square = 1'
            calculator.PropertyChanged += (sender, eventArgs) =>
            {
                if (eventArgs.PropertyName == nameof(SquareOfNumber))
                    Console.WriteLine("square = " + calculator.SquareOfNumber);
            };

            calculator.Number = 2; //Prints 'square = 4'
            calculator.Number = 3; //Prints 'square = 9'
        }
    }
}