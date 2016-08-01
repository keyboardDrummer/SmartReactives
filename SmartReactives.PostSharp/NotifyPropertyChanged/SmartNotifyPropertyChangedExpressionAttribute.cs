using System;
using System.Collections.Generic;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using PostSharp.Reflection;
using SmartReactives.Core;

namespace SmartReactives.PostSharp.NotifyPropertyChanged
{
    /// <summary>
    /// An expression property is one that has only a getter containing an expression.
    /// Place this attribute on a dependent property to make it reactive.
    /// </summary>
    [Serializable]
    [ProvideAspectRole("SmartNotifyPropertyChanged")]
    public class SmartNotifyPropertyChangedExpressionAttribute : LocationInterceptionAspect, IAspectProvider
    {
        /// <summary>
        /// Useful for debugging.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        public PropertyInfo Property { get; private set; }
        
        /// <summary>
        /// Useful for debugging.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        public IEnumerable<object> Dependents => ReactiveManager.GetDependents(this);


        /// <inheritdoc />
        public sealed override void OnGetValue(LocationInterceptionArgs args)
        {
            ReactiveManager.Evaluate(new CompositeReactiveObject(args.Instance, Property), () =>
            {
                args.ProceedGetValue();
                return true;
            });
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Sink from: " + Property.GetType().Name + "." + Property.Name;
        }

        public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
        {
            return IntroduceWeakListener.IntroduceWeakListenerForType(((LocationInfo) targetElement).DeclaringType);
        }

        /// <inheritdoc />
        public override void CompileTimeInitialize(LocationInfo targetLocation, AspectInfo aspectInfo)
        {
            Property = targetLocation.PropertyInfo;
            base.CompileTimeInitialize(targetLocation, aspectInfo);
        }
    }
}