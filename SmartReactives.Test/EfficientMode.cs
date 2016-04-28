using SmartReactives.Postsharp.NotifyPropertyChanged;
using SmartReactives.Test.Postsharp;

namespace SmartReactives.Test
{
	class EfficientMode : HasNotifyPropertyChanged
	{
		[SmartNotifyPropertyChangedVariable]
		public bool DependentWithSetter
		{
			get { return Source; }
			set { Source = value; }
		}

		[SmartNotifyPropertyChangedExpression]
		public bool Dependent => Source;

		[SmartNotifyPropertyChangedVariable]
		public bool Source { get; set; }
	}
}