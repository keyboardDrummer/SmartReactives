using SmartReactives.PostSharp.NotifyPropertyChanged;

namespace SmartReactives.Postsharp.Test
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