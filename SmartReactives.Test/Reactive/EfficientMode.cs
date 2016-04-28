using SmartReactives.Postsharp.NotifyPropertyChanged;
using SmartReactives.Test.Reactive.Postsharp;

namespace SmartReactives.Test.Reactive
{
	class EfficientMode : HasNotifyPropertyChanged
	{
		private bool _source;

		[SmartNotifyPropertyChangedVariable]
		public bool DependentWithSetter
		{
			get
			{
				return Source;
			}
			set
			{
				Source = value;
			}
		}

		[SmartNotifyPropertyChangedExpression]
		public bool Dependent
		{
			get
			{
				return Source;
			}
		}

		[SmartNotifyPropertyChangedVariable]
		public bool Source
		{
			get
			{
				return _source;
			}
			set
			{
				_source = value;
			}
		}
	}
}