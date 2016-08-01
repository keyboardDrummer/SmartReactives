using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using PostSharp.Aspects;
using PostSharp.Reflection;
using SmartReactives.Core;

namespace SmartReactives.PostSharp.NotifyPropertyChanged
{
    /// <summary>
    /// Base class for reactive aspects for a property that is a dependency of other properties.
    /// </summary>
    [Serializable]
    public abstract class SmartNotifyPropertyChangedVariableAttributeBase : LocationInterceptionAspect, IAspectProvider
    {
        protected PropertyInfo property;

        /// <inheritdoc />
        public override string ToString()
        {
            // ReSharper disable once PossibleNullReferenceException
            return "Source from: " + property.DeclaringType.Name + "." + property.Name;
        }

        /// <inheritdoc />
        public sealed override void OnSetValue(LocationInterceptionArgs args)
        {
            var newValue = args.Value;
            var oldValue = args.GetCurrentValue();

            if (Equals(newValue, oldValue))
            {
                return;
            }
            args.SetNewValue(newValue);
            ReactiveManager.WasChanged(new CompositeReactiveObject(args.Instance, property));
        }

        public override void CompileTimeInitialize(LocationInfo targetLocation, AspectInfo aspectInfo)
        {
            property = targetLocation.PropertyInfo;
            base.CompileTimeInitialize(targetLocation, aspectInfo);
        }

        public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
        {
            return IntroduceWeakListener.IntroduceWeakListenerForType(((LocationInfo)targetElement).DeclaringType);
        }
    }
}