using SmartReactives.PostSharp.NotifyPropertyChanged;

namespace SmartReactives.Postsharp.Test
{
	class HasWrapperProperty : HasNotifyPropertyChanged
    {
        [SmartNotifyPropertyChanged]
        public bool Woop { get; set; }

        public void FlipWoop()
        {
            Woop = !Woop;
        }

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
