using NUnit.Framework;
using SmartReactives.PostSharp.NotifyPropertyChanged;
using SmartReactives.Test;

// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace SmartReactives.Postsharp.Test
{
	public class ReactiveVariableTest
	{
		[Test]
		public void ForceSink()
		{
			var source = new Source();
			var badSetter = new BadSetterFixed(source);
			var counter = 0;
			badSetter.PropertyChanged += (sender, args) => counter++;
			Assert.AreEqual(source.Woop, badSetter.Bad);
			Assert.AreEqual(0, counter);
			source.FlipWoop();
			Assert.AreEqual(1, counter);
		}

		class BadSetterFixed : HasNotifyPropertyChanged
		{
			readonly Source source;

			public BadSetterFixed(Source source)
			{
				this.source = source;
			}

			[SmartNotifyPropertyChanged]
			public bool Bad
			{
				get { return source.Woop; }
				set { source.Woop = value; }
			}
		}
	}
}