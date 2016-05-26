# SmartReactives [![Join the chat at https://gitter.im/keyboardDrummer/SmartReactives](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/keyboardDrummer/SmartReactives.rx?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

SmartReactives is a .NET library that automatically discovers dependencies between expressions and variables. Specifying such dependencies is a common problem in software, related to updating a user interface, clearing stale caches, and more.

##### Example
```c#
var input = new ReactiveVariable<int>(1);
var square = new ReactiveExpression<int>(() => input.Value * input.Value);
square.Subscribe(getSquare => Console.WriteLine("square = " + getSquare())); //Prints 'square = 1'

input.Value = 2; //Prints 'square = 4'
input.Value = 3; //Prints 'square = 9'
```

SmartReactives is inspired by [Scala.Rx](https://github.com/lihaoyi/scala.rx), which was inspired by the paper [Deprecating the Observer Pattern](https://scholar.google.nl/scholar?q=deprecating+the+observer+pattern&btnG=&hl=en&as_sdt=0%2C5), by Odersky.

To start using SmartReactives simply add the NuGet package SmartReactives to your project. Also add SmartReactives.PostSharp if you're using PostSharp.

#Examples

## Basic functionality
This example demonstrates the basic functionality of SmartReactives using the classes ReactiveVariable and ReactiveExpression.
```c#
var input = new ReactiveVariable<int>(1);
var square = new ReactiveExpression<int>(() => input.Value * input.Value);
square.Subscribe(getSquare => Console.WriteLine("square = " + getSquare())); //Prints 'square = 1'

input.Value = 2; //Prints 'square = 4'
input.Value = 3; //Prints 'square = 9'
```
Output:
```
square = 1
square = 4
square = 9
```
Note that even though square uses the input value twice, we only get one notification per change in input.

## Recursive

## Precise
In the following example, the expression leftOrRight only depends on variable right when variable left is false, since we are using the lazy or operator ||. 
If we change right while left is false, then we don't get any updates from leftOrRight. 
In general, SmartReactives won't give you any updates for old dependencies or possible future dependencies.
Note that if we only want to get updates if leftOrRight changes then we can use 'leftOrRight.DistinctUntilChanged().Subscribe(...)'.

```c#
var left = new ReactiveVariable<bool>();
var right = new ReactiveVariable<bool>();
var leftOrRight = new ReactiveExpression<bool>(() => left.Value || right.Value);
leftOrRight.Subscribe(getValue => Console.WriteLine("leftOrRight = " + getValue())); // Prints 'leftOrRight = false'

right.Value = true; // Prints 'leftOrRight = true'
left.Value = true; // Prints 'leftOrRight = true'
right.Value = false; // Prints nothing
```

## ReactiveCache
This example shows how to use ReactiveCache to get a cache which automatically clears itself when it becomes stale.
```c#
var input = new ReactiveVariable<int>(2); //We define a reactive variable.
Func<int> f = () => //f is the calculation we want to cache.
{
    Console.WriteLine("f was evaluated");
    return input.Value * input.Value; //f depends on the reactive variable 'input'.
};
var cache = new ReactiveCache<int>(f); //We base our cache on f.

Console.WriteLine("f() = " + cache.Get()); //Cache miss so we evaluate f.
Console.WriteLine("f() = " + cache.Get()); //Cache hit so we don't evaluate f.

input.Value = 3; //We change our input variable, causing our cache to become stale.
Console.WriteLine("f() = " + cache.Get()); //Cache miss, so we evaluate f.
```
Output:
```
f was evaluated
f() = 4
f() = 4
f was evaluated
f() = 9
```
			
## Reactive Properties
This examples demonstrates two methods to implement a reactive property. The first method uses the class ReactiveVariable that we already know as a backing field for our reactive property.
The second method applies ReactiveVariableAttribute to the property, which in combination with PostSharp does all the work.
```c#
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
		multiplication.Subscribe(getMultiplication => Console.WriteLine("multiplication = " + getMultiplication())); //Prints 'multiplication = 1'
		UsingAnAttributeAndPostSharp = 2; //Prints 'multiplication = 2'
		UsingABackingField = 2; //Prints 'multiplication = 4'
	}
}
```

## ReactiveCacheAttribute
The bottom example demonstrates using ReactiveVariableAttribute and ReactiveCacheAttribute to effortlessly setup a cache. 
The calculation in the example has dependencies that change during runtime, so it's not statically known which variable changes will cause the cache to become stale.
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

		Assert.AreEqual(9, calculator.Square); // Cache miss. Prints 'cache miss'
		Assert.AreEqual(9, calculator.Square); // Cache hit.
	}
}
```

## SmartNotifyPropertyChanged
Implementing PropertyChanged for a property is a known cause for boilerplate. PostSharp allows you to remove this boilerplate using its attribute NotifyPropertyChanged.
However, sometimes a property A depends on another property B. In this case we would like both properties to call PropertyChanged when B changes. 
The PostSharp attribute NotifyPropertyChanged won't do this, but SmartNotifyPropertyChanged will. This below example demonstrates this.

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
        
        Console.WriteLine("square = " + calculator.SquareOfNumber); // Prints 'square = 1'
        calculator.PropertyChanged += (sender, eventArgs) =>
        {
            if (eventArgs.PropertyName == nameof(SquareOfNumber))
                Console.WriteLine("square = " + calculator.SquareOfNumber);
        };

        calculator.Number = 2; // Prints 'square = 4'
        calculator.Number = 3;; //Prints 'square = 9'
    }
}
```

Output:
```
square = 1
square = 4
square = 9
```

# Documentation

The SmartReactives API is divided into three layers:
- Common: the bread and butter of SmartReactives. The central classes are ReactiveVariable and ReactiveExpression.
- Core: the lowest level API on which the other API's are based. The central class here is ReactiveManager. In 99% of the cases you won't have a reason for using the Core API.
- Postsharp: an API of attributes that when put on properties will enhance them with SmartReactive capabilities. This API provides the most consise code.

##  Core

The basic premise on which SmartReactives is built is quite simple: if we're evaluating A and suddenly B is read from, then A must depend on B. The tricky part is correctly dealing with multiple threads, not leaking any memory, being performant, and making sure we don't notify too often or too little.

The Core API of SmartReactives can be accessed from ReactiveManager, which exposes these methods:

```C#
Evaluate<T>(IListener dependent, Func<T> func)
void WasRead(object source)
void WasChanged(object source)
```

Each of these methods requires you to pass an object to ReactiveManager. We call these reactive objects. By correctly calling *Evaluate* and *WasRead*, ReactiveManager will build a dependency graph of your reactive objects. Although ReactiveManager remembers which objects you pass to it, it will never leak memory for those objects. We use WeakReference and ConditionalWeakTable to only hold weak references to any reactive objects.

When you call *WasChanged*, ReactiveManager will notify all objects that depend on the changed object. 

An important rule of ReactiveManager is the following: after an object is notified by the ReactiveManager, it will not be notified again until the object is evaluated again. Every time an object changes its value and you're still interesting in this object, then you must evaluate it again to indicate this interest. Simply put, if you've lost interest in an object, then we won't bother you when it changes. Internally this behavior is required because an object's dependencies my change after the object changes its value, and we must re-evaluate the object to again correctly establish its dependencies. To prevent doing any unnecessary work, we leave it up to the user to re-evaluate the object.
