using NUnit.Framework;
using SmartReactives.Postsharp.NotifyPropertyChanged;

namespace SmartReactives.Examples
{
	class Person : HasNotifyPropertyChanged
	{
		[SmartNotifyPropertyChanged]
		public int LengthInCentimeters { get; set; }

		[SmartNotifyPropertyChanged]
		public bool TallEnoughForTheRollerCoaster => LengthInCentimeters > 140;
	}

	class Tests
	{
		[Test]
		public void TallEnoughDependsOnLength()
		{
			int tallChangedCounter = 0;

			var person = new Person();
			person.LengthInCentimeters = 120;

			person.PropertyChanged += (sender, eventArgs) =>
			{
				if (eventArgs.PropertyName == nameof(Person.TallEnoughForTheRollerCoaster))
					tallChangedCounter++;
			};

			person.LengthInCentimeters = 180;

			Assert.AreEqual(1, tallChangedCounter);
		}
	}
}
