# SmartReactives

SmartReactives is a .NET library that will automatically discover dependencies between expressions and variables. Forget about manually specifying data bindings and clearing stale caches: SmartReactives will do this for you.

SmartReactives is inspired by [Scala.Rx](https://github.com/lihaoyi/scala.rx), which was inspired by the paper [Deprecating the Observer Pattern](https://scholar.google.nl/scholar?q=deprecating+the+observer+pattern&btnG=&hl=en&as_sdt=0%2C5), by Odersky.

To start using SmartReactives simple add the NuGet package to your project.

#Examples

## Basic usage
This example demonstrates the basic functionality of SmartReactives using the primitives ReactiveVariable and ReactiveExpression.
```c#
var input = new ReactiveVariable<int>(1);
var square = new ReactiveExpression<int>(() => input.Value * input.Value);
square.Subscribe(getSquare => Console.WriteLine("square = " + getSquare()));

input.Value = 2;
input.Value = 3;
```
Output:
```
square = 1
square = 4
square = 9
```

## Automatic cache clearing
This example shows how to use ReactiveCache to get a cache which automatically clears itself when it becomes stale.
```c#
var input = new ReactiveVariable<int>(2); //We define a reactive variable.
Func<int> f = () => //f is the calculation we want to cache.
{
    Console.WriteLine("f was evaluated");
    return input.Value * input.Value; //f depends on our reactive variable input.
};
var cache = new ReactiveCache<int>(f); //We base our cache on f.

Console.WriteLine("f() = " + cache.Get()); //Cache was not set so we evaluate f.
Console.WriteLine("f() = " + cache.Get()); //Cache is set so we don't evaluate f.

input.Value = 3; //We change our input variable, causing our cache to become stale.
Console.WriteLine("f() = " + cache.Get()); //Cache is stale, so we must evaluate f.
```
Output:
```
f was evaluated
f() = 4
f() = 4
f was evaluated
f() = 9
```


## Automatic NotifyPropertyChanged
Using Postsharp, is it possible to automatically call PropertyChanged for a property, when that property changes. However, sometimes a property A depends on another property B. In this case we would like both properties to call PropertyChanged when B changes. This example shows how SmartReactives will do this for you.

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
```

Output:
```
square = 4
square = 9
square = 16
square = 25
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
