using SmartReactives.Postsharp.NotifyPropertyChanged;
using SmartReactives.Test.Reactive.Postsharp;

namespace SmartReactives.Test.Reactive
{
	class AutomaticMode : HasNotifyPropertyChanged
	{
		[SmartNotifyPropertyChanged]
		public bool DependentWithSetter
		{
			get { return Source; }
			set { Source = value; }
		}

		[SmartNotifyPropertyChanged]
		public bool Dependent => Source;

		[SmartNotifyPropertyChanged]
		public bool Source { get; set; }
	}
}