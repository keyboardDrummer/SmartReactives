# SmartReactives

SmartReactives is a .NET library that will automatically discover dependencies between expressions and variables for you. Forget about manually specifying data bindings and clearing stale caches: SmartReactives will do this for you.

SmartReactives is inspired by [Scala.Rx](https://github.com/lihaoyi/scala.rx), which was inspired by the paper [Deprecating the Observer Pattern](https://scholar.google.nl/scholar?q=deprecating+the+observer+pattern&btnG=&hl=en&as_sdt=0%2C5), by Odersky.

The SmartReactives API is divided into three layers:
- Common: the bread and butter of SmartReactives. The central classes are ReactiveVariable and ReactiveExpression.
- Core: a lower level API on which the other API's are based. The central class here is ReactiveManager.
- Postsharp: an API of attributes that when put on properties will enhance them with SmartReactive capabilities. This API provides the most consise code.

#Examples

## Basic usage
```c#
public void SquareInput()
{
    var input = new ReactiveVariable<int>(1);
    var square = new ReactiveExpression<int>(() => input.Value * input.Value);
    square.Subscribe(getSquare => Console.WriteLine("square = " + getSquare()));

    input.Value = 2;
    input.Value = 3;
}
```
Output:
```
square = 1
square = 4
square = 9
```

## Automatic cache clearing
Here is an example that shows how to use ReactiveCache to get a cache which automatically clears itself when it becomes stale:
```c#
public void Cache()
{
    var input = new ReactiveVariable<int>(1); //We define a reactive variable.
    Func<int> f = () => //f is the calculation we want to cache.
    {
        Console.WriteLine("f was evaluated");
        return 3 * input.Value; //f depends on our reactive variable input.
    };
    var cache = new ReactiveCache<int>(f); //We base our cache on f.

    Console.WriteLine("f() = " + cache.Get()); //Cache was not set so we evaluate f.
    Console.WriteLine("f() = " + cache.Get()); //Cache is set so we don't evaluate f.

    input.Value = 2; //We change our input variable, causing our cache to become stale.
    Console.WriteLine("f() = " + cache.Get()); //Cache is stale, so we must evaluate f.
}
```
Output:
```
f was evaluated
f() = 3
f() = 3
f was evaluated
f() = 6
```


## Automatic NotifyPropertyChanged
Below is an example that shows how SmartReactives will automatically call PropertyChanged for properties even when their value has changed indirectly, meaning by a change in an underlying property. 

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
