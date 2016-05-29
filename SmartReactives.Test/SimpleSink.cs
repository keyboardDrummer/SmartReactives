using SmartReactives.Extensions;
using SmartReactives.Postsharp.Test;

namespace SmartReactives.Test
{
    class SimpleSink //: HasNotifyPropertyChanged
    {
        readonly ReactiveVariable<Source> source = new ReactiveVariable<Source>();

        public SimpleSink(Source source)
        {
            Source = source;
            _bool = new ReactiveExpression<bool>(() => Source.Woop);
        }

        public Source Source
        {
            get { return source.Value; }
            set { source.Value = value; }
        }

        readonly ReactiveExpression<bool> _bool;
        public bool Boop => _bool.Evaluate();

        public ReactiveExpression<bool> BoopReactive => _bool;
    }
}