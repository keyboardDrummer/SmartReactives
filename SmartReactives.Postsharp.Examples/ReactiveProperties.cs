using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmartReactives.Common;
using SmartReactives.PostSharp;

namespace SmartReactives.PostsharpExamples
{
    class ReactiveProperties
    {
        readonly ReactiveVariable<int> usingABackingField = Reactive.Variable(1);

        int UsingABackingField
        {
            get { return usingABackingField.Value; }
            set { usingABackingField.Value = value; }
        }

        [ReactiveVariable]
        int UsingAnAttributeAndPostSharp { get; set; } = 1;

        public void Test()
        {
            var product = Reactive.Expression(() => UsingABackingField * UsingAnAttributeAndPostSharp);
            product.Subscribe(getProduct => Console.WriteLine("product = " + getProduct())); //Prints 'multiplication = 1'
            UsingAnAttributeAndPostSharp = 2; //Prints 'multiplication = 2'
            UsingABackingField = 2; //Prints 'multiplication = 4'
        }
    }
}