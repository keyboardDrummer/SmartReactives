using SmartReactives.Common;

namespace SmartReactives.Test
{
    class SinkChain //: HasNotifyPropertyChanged
    {
        readonly ReactiveVariable<bool> source = new ReactiveVariable<bool>();

        public SinkChain()
        {
            InnerSinkReactive = new ReactiveExpression<bool>(() => !Source);
            OuterSinkReactive = new ReactiveExpression<bool>(() => !InnerSink);
        }

        public bool Source
        {
            get { return source.Value; }
            set { source.Value = value; }
        }

        public bool InnerSink => InnerSinkReactive.Evaluate();

        public bool OuterSink => OuterSinkReactive.Evaluate();

        public ReactiveExpression<bool> OuterSinkReactive { get; }

        public ReactiveExpression<bool> InnerSinkReactive { get; }
    }
}