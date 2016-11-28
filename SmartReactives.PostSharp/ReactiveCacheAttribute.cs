using System;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using SmartReactives.Common;
using SmartReactives.Core;

namespace SmartReactives.PostSharp
{
    /// <summary>
    /// Adds a reactive cache to the applied property.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property)]
    public class ReactiveCacheAttribute : LocationInterceptionAspect, IInstanceScopedAspect, IListener
    {
        public object Value { get; set; }

        public bool IsSet { get; set; }
        
        public object CreateInstance(AdviceArgs adviceArgs)
        {
            return new ReactiveCacheAttribute();
        }

        public void RuntimeInitializeInstance()
        {
        }

        public sealed override void OnGetValue(LocationInterceptionArgs args)
        {
            if (IsSet)
            {
                ReactiveManager.WasRead(this);
                args.Value = Value;
            }
            else
            {
                Value = ReactiveManager.Evaluate(this, () =>
                {
                    args.ProceedGetValue();
                    return args.Value;
                });
                IsSet = true;
            }
        }

        public void Notify()
        {
            IsSet = false;
        }

	    public bool StrongReference => false;
    }
}