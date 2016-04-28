using SmartReactives.Postsharp.NotifyPropertyChanged;

namespace SmartReactives.Test.Reactive.Postsharp
{
	class HasWrapperProperty : Source
	{
		[SmartNotifyPropertyChanged]
		public bool Wrapper
		{
			get
			{
				return Woop;
			}
			set
			{
				Woop = value;
			}
		}
	}
}
