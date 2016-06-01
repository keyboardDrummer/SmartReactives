using SmartReactives.Common;

namespace SmartReactives.Test
{
    class SimpleSink //: HasNotifyPropertyChanged
    {
        readonly ReactiveVariable<Source> source = new ReactiveVariable<Source>();

        public SimpleSink(Source source)
        {
            Source = source;
            BoopReactive = new ReactiveExpression<bool>(() => Source.Woop);
        }

        public Source Source
        {
            get { return source.Value; }
            set { source.Value = value; }
        }

        public bool Boop => BoopReactive.Evaluate();

        public ReactiveExpression<bool> BoopReactive { get; }
    }
}