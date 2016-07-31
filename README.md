# SmartReactives [![Join the chat at https://gitter.im/keyboardDrummer/SmartReactives](https://badges.gitter.im/keyboardDrummer/SmartReactives.svg)](https://gitter.im/keyboardDrummer/SmartReactives?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Build status](https://ci.appveyor.com/api/projects/status/hpigcvs8bnrkdamh?svg=true)](https://ci.appveyor.com/project/keyboardDrummer/smartreactives)

SmartReactives is an extension to Rx.NET that detects when an expression changes its value and exposes these changes as an IObservable. Detecting changes in expressions is a common problem, related to user interfaces, caching, validation, and much more.

This example demonstrates the basic functionality:
```c#
var input = Reactive.Variable(1);
var inputSquared = Reactive.Expression(() => input * input);
inputSquared.Subscribe(getSquare => Console.WriteLine("square = " + getSquare())); //Prints 'square = 1'

input.Value = 2; //Prints 'square = 4'
input.Value = 3; //Prints 'square = 9'
```
```Reactive.Expression``` returns a ```ReactiveExpression<T>``` which implements ```IObservable<Func<T>>``` so we can subscribe to it. The function ```getSquare``` is simply a shortcut to ```ReactiveExpression<T>.Evaluate```, which evaluates the function you created the reactive expression with, namely ```() => input * input```.

To start using SmartReactives simply add the NuGet package [SmartReactives](https://www.nuget.org/packages/SmartReactives/) to your project. Also add [SmartReactives.PostSharp](https://www.nuget.org/packages/SmartReactives.PostSharp/) if you're using PostSharp.

If you're looking for something like SmartReactives but outside of .NET then take a look at these projects:
- Scala: [Scala.Rx](https://github.com/lihaoyi/scala.rx)
- JavaScript: [Mobx](https://github.com/mobxjs/mobx) and [Meteor Tracker](https://atmospherejs.com/meteor/tracker)
- OCaml: [Incremental](https://github.com/janestreet/incremental)

## Examples
This section demonstrates the functionality of SmartReactives by showing a number of examples.

### Caching
This example shows how to use Reactive.Cache to get a cache which automatically clears itself when it becomes stale.
```c#
var input = Reactive.Variable(2); //We define a reactive variable.
Func<int> f = () => //f is the calculation we want to cache.
{
    Console.WriteLine("cache miss");
    return input * input; //f depends on our reactive variable 'input'.
};
var cache = Reactive.Cache(f); //We base our cache on f.

Assert.AreEqual(4, cache.Get()); //Prints 'cache miss'
Assert.AreEqual(4, cache.Get()); //Cache hit.

input.Value = 3; //Cache becomes stale.

Assert.AreEqual(9, cache.Get()); //Prints 'cache miss'
Assert.AreEqual(9, cache.Get()); //Cache hit.
```

### Collections
This example shows off how to make an ```IList<T>``` reactive by calling ToReactive on it. The reactive list is precise: if you access an index you will only get an update if that particular index changes.

```c#
var reactiveList = new List<int> { 0, 1, 2 }.ToReactive();
var elementAtIndex1 = Reactive.Expression(() => reactiveList[1]);

//Prints 'item at index 2 changed to 1'
elementAtIndex1.Subscribe(getValue => Console.WriteLine("item at index 1 changed to " + getValue())); 

reactiveList[1] = 3; //Prints 'item at index 1 changed to 3'
reactiveList[2] = 4; //Prints nothing
reactiveList.Add(5); //Prints nothing
reactiveList.Insert(0, 6); //Prints 'item at index 1 changed to 0'
```

Next to ```IList<T>``` there are also reactive collections for ```ISet<T>``` and ```IDictionary<T,U>```.

### Composition
This example shows that a ReactiveExpression can refer to other ReactiveExpressions. In this way you can build arbitrary graphs of reactive objects. 
The example demonstrates a graph in the shape of a diamond.  

```c#
var input = Reactive.Variable(1);
var timesTwo = Reactive.Expression(() => input * 2);
var timesThree = Reactive.Expression(() => input * 3);
var sumOfBoth = Reactive.Expression(() => timesTwo.Evaluate() + timesThree.Evaluate());
sumOfBoth.Subscribe(getValue => Console.WriteLine("sumOfBoth = " + getValue())); //Prints 'sumOfBoth = 5'
input.Value = 2; //Prints 'sumOfBoth = 10'
input.Value = 3; //Prints 'sumOfBoth = 15'
```

Note that although the input has two paths in the graph to sumOfBoth, there is only one notification for sumOfBoth when input changes. SmartReactives makes sure to notify an expression once and only once when it changes value.

### Precise
In the following example, the expression leftOrRight only depends on variable right when variable left is false, since we are using the lazy or operator ||. 
If we change right while left is false, then we don't get any updates from leftOrRight. 
In general, SmartReactives won't give you any updates for old dependencies or possible future dependencies.
Note that if we only want to get updates if leftOrRight changes then we can use ```leftOrRight.DistinctUntilChanged().Subscribe(...)```.

```c#
var left = Reactive.Variable(false);
var right = Reactive.Variable(false);
var leftOrRight = Reactive.Expression(() => left || right);
leftOrRight.Subscribe(getValue => Console.WriteLine("leftOrRight = " + getValue())); //Prints 'leftOrRight = False'

right.Value = true; //Prints 'leftOrRight = True'
left.Value = true; //Prints 'leftOrRight = True'
right.Value = false; //Prints nothing
```

### Properties
This examples demonstrates two methods to implement a reactive property. The first method uses the class ReactiveVariable that we already know as a backing field for our reactive property.
The second method applies ReactiveVariableAttribute to the property, which in combination with PostSharp does all the work.
```c#
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
		product.Subscribe(getProduct => Console.WriteLine("product = " + getProduct())); //Prints 'product = 1'
        UsingAnAttributeAndPostSharp = 2; //Prints 'product = 2'
        UsingABackingField = 2; //Prints 'product = 4'
    }
}
```

### ReactiveCacheAttribute
The following example demonstrates using ReactiveVariableAttribute and ReactiveCacheAttribute to effortlessly setup a cache.
```c#
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

		Assert.AreEqual(9, calculator.Square); //Cache miss. Prints 'cache miss'
		Assert.AreEqual(9, calculator.Square); //Cache hit.
	}
}
```

### NotifyPropertyChanged
Implementing ```PropertyChanged``` for a property is a known cause for boilerplate. PostSharp allows you to remove this boilerplate using its attribute ```NotifyPropertyChanged```.
However, sometimes a property A depends on another property B. In this case we would like both properties to call PropertyChanged when B changes. 
The PostSharp attribute ```NotifyPropertyChanged``` won't do this, but ```SmartNotifyPropertyChanged``` will, as shown in the following example.

```c#
class Calculator : HasNotifyPropertyChanged
{
    [SmartNotifyPropertyChanged]
    public int Number { get; set; }

    [SmartNotifyPropertyChanged]
    public int SquareOfNumber => Number * Number;

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
        calculator.Number = 3;; //Prints 'square = 9'
    }
}
```
