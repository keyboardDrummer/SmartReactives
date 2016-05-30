using SmartReactives.Common;

namespace SmartReactives.Test
{
    class ComplexSink //: HasNotifyPropertyChanged
    {
        readonly ReactiveVariable<Source> source = new ReactiveVariable<Source>();
        readonly ReactiveVariable<Source> source2 = new ReactiveVariable<Source>();
        readonly ReactiveExpression<string> complex;

        public ComplexSink(Source source, Source source2)
        {
            this.source.Value = source;
            Source2 = source2;
            complex = new ReactiveExpression<string>(() => Source.Woop ? "foo" : (Source2.Woop ? "bar" : "zoo"));
        }
        
        public Source Source => source.Value;

        public Source Source2
        {
            get { return source2.Value; }
            set { source2.Value = value; }
        }
        
        public string Complex => complex.Evaluate();
    }
}