using System.Collections.Generic;
using System.Linq;
using SmartReactives.Extensions;

namespace SmartReactives.Test
{
    class Tree //: HasNotifyPropertyChanged
    {
        readonly ReactiveVariable<IList<Tree>> children = new ReactiveVariable<IList<Tree>>();
        readonly ReactiveVariable<bool> successful = new ReactiveVariable<bool>();
        readonly ReactiveExpression<bool> hasSuccessfulBloodReactive;

        public Tree(IList<Tree> children = null, bool successful = false)
        {
            this.children.Value = children ?? new List<Tree>();
            Successful = successful;
            hasSuccessfulBloodReactive = new ReactiveExpression<bool>(() => Successful || Children.Any(child => child.HasSuccessfulBlood));
        }

        public IList<Tree> Children => children.Value;

        public bool Successful
        {
            get { return successful.Value; }
            set { successful.Value = value; }
        }
        
        public bool HasSuccessfulBlood => hasSuccessfulBloodReactive.Evaluate();

        public ReactiveExpression<bool> HasSuccessfulBloodReactive => hasSuccessfulBloodReactive;
    }
}