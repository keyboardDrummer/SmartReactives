using SmartReactives.Postsharp.NotifyPropertyChanged;
using SmartReactives.Test.Postsharp;

namespace SmartReactives.Test
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
