using System;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using SmartReactives.Core;

namespace SmartReactives.PostSharp.NotifyPropertyChanged
{
    /// <summary>
    /// Makes forwarder properties reactive.
    /// A forwarder property wraps around another property by having both a getter and setter that refer to the underlying property.
    /// </summary>
    [ProvideAspectRole("SmartNotifyPropertyChanged")]
    [Serializable]
    [AttributeUsage(AttributeTargets.Property)]
    public class SmartNotifyPropertyChangedAttribute : SmartNotifyPropertyChangedVariableAttributeBase
    {
        public sealed override void OnGetValue(LocationInterceptionArgs args)
        {
            ReactiveManager.Evaluate(new CompositeReactiveObject(args.Instance, property), () =>
            {
                args.ProceedGetValue();
                return true;
            });
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Sink from: " + property.GetType().Name + "." + property.Name;
        }
    }
}