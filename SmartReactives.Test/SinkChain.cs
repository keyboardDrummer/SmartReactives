using SmartReactives.Extensions;

namespace SmartReactives.Test
{
    class SinkChain //: HasNotifyPropertyChanged
    {
        readonly ReactiveVariable<bool> source = new ReactiveVariable<bool>();
        readonly ReactiveExpression<bool> innerSink;
        readonly ReactiveExpression<bool> outerSink;

        public SinkChain()
        {
            innerSink = new ReactiveExpression<bool>(() => !Source);
            outerSink = new ReactiveExpression<bool>(() => !InnerSink);
        }

        public bool Source
        {
            get { return source.Value; }
            set { source.Value = value; }
        }

        public bool InnerSink => innerSink.Evaluate();
            
        public bool OuterSink => outerSink.Evaluate();

        public ReactiveExpression<bool> OuterSinkReactive => outerSink;
        public ReactiveExpression<bool> InnerSinkReactive => innerSink;
    }
}