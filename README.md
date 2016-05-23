# SmartReactives

SmartReactives is a .NET library that will automatically discover dependencies between expressions and variables for you. Forget about manually specifying data bindings and clearing stale caches: SmartReactives will do this for you.

SmartReactives is inspired by [Scala.Rx](https://github.com/lihaoyi/scala.rx), which was inspired by the paper [Deprecating the observer pattern, by Odersky](https://scholar.google.nl/scholar?q=deprecating+the+observer+pattern&btnG=&hl=en&as_sdt=0%2C5).

#Examples

## Automatic NotifyPropertyChanged
Below is an example that shows how SmartReactives will automatically call PropertyChanged for properties even when their value has changed indirectly, meaning by a change in an underlying property. 

```c#
class Person : HasNotifyPropertyChanged
{
	[SmartNotifyPropertyChanged]
	public int LengthInCentimeters { get; set; }

	[SmartNotifyPropertyChanged]
	public bool TallEnoughForTheRollerCoaster => LengthInCentimeters > 140;

    [Test]
    public static void TallEnoughDependsOnLengthProperties()
    {
        var tallEnoughChangedCounter = 0;

        var person = new Person();
        person.LengthInCentimeters = 120;

        person.PropertyChanged += (sender, eventArgs) =>
        {
            if (eventArgs.PropertyName == nameof(TallEnoughForTheRollerCoaster))
                tallEnoughChangedCounter++;
        };

        Assert.AreEqual(false, person.TallEnoughForTheRollerCoaster);
        person.LengthInCentimeters = 180;
        Assert.AreEqual(true, person.TallEnoughForTheRollerCoaster);

        Assert.AreEqual(1, tallEnoughChangedCounter);
    }
}
```

## Automatic cache clearing
Here is an example that shows how to use ReactiveCache to get a cache which automatically clears itself when it becomes stale:
```c#
[Test]
public void Cache()
{
    var input = new ReactiveVariable<int>(1); //We define a reactive variable.
    var evaluationCounter = 0;
    Func<int> f = () => //f is the calculation we want to cache.
    {
        evaluationCounter++; //For testing we want to track evaluations.
        return input.Value*3; //f depends on our reactive variable input.
    };
    var cache = new ReactiveCache<int>(f); //We base our cache on f.

    Assert.AreEqual(3, cache.Get()); //Cache was not set so we evaluate f.
    Assert.AreEqual(1, evaluationCounter); //f was evaluated.

    Assert.AreEqual(3, cache.Get()); //Cache is set so we don't evaluate f.
    Assert.AreEqual(1, evaluationCounter); //f was not evaluated.

    input.Value = 2; //We change our input variable, causing our cache to become stale.
    Assert.AreEqual(6, cache.Get()); //Cache is stale, so we must evaluate f.
    Assert.AreEqual(2, evaluationCounter); //f was evaluated.
}
```
