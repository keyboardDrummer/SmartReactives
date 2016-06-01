using SmartReactives.PostSharp.NotifyPropertyChanged;

namespace SmartReactives.Postsharp.Test
{
    class HasWrapperProperty : HasNotifyPropertyChanged
    {
        [SmartNotifyPropertyChanged]
        public bool Woop { get; set; }

        [SmartNotifyPropertyChanged]
        public bool Wrapper
        {
            get { return Woop; }
            set { Woop = value; }
        }

        public void FlipWoop()
        {
            Woop = !Woop;
        }
    }
}