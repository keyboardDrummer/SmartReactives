using SmartReactives.PostSharp.NotifyPropertyChanged;

namespace SmartReactives.Postsharp.Test
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
