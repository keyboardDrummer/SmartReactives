using SmartReactives.Postsharp.NotifyPropertyChanged;
using SmartReactives.Test.Reactive.Postsharp;

namespace SmartReactives.Test.Reactive
{
	public class Source : HasNotifyPropertyChanged
	{
		[SmartNotifyPropertyChanged]
		public bool Woop
		{
			get;
			set;
		}

		public void FlipWoop()
		{
			Woop = !Woop;
		}
	}
}
