using SmartReactives.PostSharp.NotifyPropertyChanged;

namespace SmartReactives.Postsharp.Test
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
