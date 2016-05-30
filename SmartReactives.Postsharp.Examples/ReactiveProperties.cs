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
		readonly ReactiveVariable<int> usingABackingField = new ReactiveVariable<int>(1);
		int UsingABackingField
		{
			get { return usingABackingField.Value; }
			set { usingABackingField.Value = value; }
		}

		[ReactiveVariable]
		int UsingAnAttributeAndPostSharp { get; set; } = 1;

		[Test]
		public void Test()
		{
			var multiplication = new ReactiveExpression<int>(() => UsingABackingField * UsingAnAttributeAndPostSharp);
			multiplication.Subscribe(getMultiplication => Console.WriteLine("multiplication = " + getMultiplication()));
			UsingAnAttributeAndPostSharp = 2;
			UsingABackingField = 2;
		}
	}
}
