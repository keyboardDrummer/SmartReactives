using System;
using NUnit.Framework;
using SmartReactives.Test;

namespace SmartReactives.Postsharp.Test
{



    public class ReactiveAttributesTest
	{
		[Test]
		public void EfficientModeDependent()
		{
			var efficientMode = new EfficientMode();
			var counter = 0;

			ObservableUtility.FromProperty(() => efficientMode.Dependent).Subscribe(_ => counter++);

			Assert.AreEqual(1, counter);
			efficientMode.Source = !efficientMode.Source;

			Assert.AreEqual(2, counter);
		}

		[Test]
		public void EfficientModeDependentWithSetter()
		{
			var efficientMode = new EfficientMode();
			var counter = 0;
			ObservableUtility.FromProperty(() => efficientMode.DependentWithSetter).Subscribe(_ => counter++);

			Assert.AreEqual(1, counter);
			efficientMode.Source = !efficientMode.Source;

			Assert.AreEqual(1, counter);
		}

		[Test]
		public void AutomaticModeDependent()
		{
			var efficientMode = new AutomaticMode();
			var counter = 0;
			ObservableUtility.FromProperty(() => efficientMode.Dependent).Subscribe(_ => counter++);

			Assert.AreEqual(1, counter);
			efficientMode.Source = !efficientMode.Source;

			Assert.AreEqual(2, counter);
		}

		[Test]
		public void AutomaticModeDependentWithSetter()
		{
			var efficientMode = new AutomaticMode();
			var counter = 0;
			ObservableUtility.FromProperty(() => efficientMode.DependentWithSetter).Subscribe(_ => counter++);

			Assert.AreEqual(1, counter);
			efficientMode.Source = !efficientMode.Source;
			Assert.AreEqual(2, counter);
		}
	}
}
