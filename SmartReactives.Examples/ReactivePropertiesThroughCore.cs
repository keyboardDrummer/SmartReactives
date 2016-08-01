using System;
using SmartReactives.Common;
using SmartReactives.Core;

namespace SmartReactives.Examples
{
	class ReactivePropertiesThroughCore
	{
		int input = 1;
		public int Input
		{
			get
			{
				ReactiveManager.WasRead(new CompositeReactiveObject(this, nameof(Input)));
				return input;
			}
			set
			{
				input = value;
				ReactiveManager.WasChanged(new CompositeReactiveObject(this, nameof(Input)));
			}
		}

		public void Test()
		{
			var inputSquared = Reactive.Expression(() => Input * Input);
			inputSquared.Subscribe(getSquare => Console.WriteLine("square = " + getSquare())); //Prints 'square = 1'
			Input = 2; //Prints 'square = 2'
		}
	}
}