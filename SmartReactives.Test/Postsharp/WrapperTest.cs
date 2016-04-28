using System;
using System.Reactive.Linq;
using NUnit.Framework;
using SmartReactives.Postsharp.NotifyPropertyChanged;

namespace SmartReactives.Test.Postsharp
{
	class WrapperTest
	{
		const int NotificationsWhenSettingDependentSetter = 2; //Ideally 1
		readonly bool FindsChangingDependenciesForDependentWithSetter = false; //false is fine. This scenario only occurs when someone creates an inconsistent property, where the property does not necessarily have the value you last set it to.

		[Test]
		public void DependentWithSetter()
		{
			var dependent = new HasWrapperProperty();
			var counter = 0;
			ObservableUtility.FromProperty(() => dependent.Wrapper).Skip(1).Subscribe(_ => counter++);
			var expectation = 0;

			Assert.AreEqual(dependent.Woop, dependent.Wrapper);
			Assert.AreEqual(expectation, counter);
			dependent.FlipWoop();
			Assert.AreEqual(++expectation, counter);

			dependent.Wrapper = !dependent.Wrapper;
			Assert.AreEqual(expectation += NotificationsWhenSettingDependentSetter, counter);
		}

		[Test]
		public void TestChangingDependenciesForSourceTrap()
		{
			var first = new Source();
			var second = new Source();
			var sink = new HasAsymmetricalWrapper(first, second);
			var counter = 0;
			var expectation = 0;
			Assert.AreEqual(false, sink.AsymmetricalWrapper);
			first.Woop = true;
			ObservableUtility.FromProperty(() => sink.AsymmetricalWrapper, false).Subscribe(value => { counter++; });
			Assert.AreEqual(expectation, counter);
			second.Woop = true;
			Assert.AreEqual(expectation += FindsChangingDependenciesForDependentWithSetter ? 1 : 0, counter);
		}

		class HasAsymmetricalWrapper : HasNotifyPropertyChanged
		{
			readonly Source source1;
			readonly Source source2;

			public HasAsymmetricalWrapper(Source source1, Source source2)
			{
				this.source1 = source1;
				this.source2 = source2;
			}

			[SmartNotifyPropertyChanged]
			public bool AsymmetricalWrapper
			{
				get { return source1.Woop && source2.Woop; }
				set { source1.Woop = value; }
			}
		}
	}
}