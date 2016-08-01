using System;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Reflection;
using SmartReactives.Core;

namespace SmartReactives.PostSharp
{
    /// <summary>
    /// Allows this property to be used by the reactive framework.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property)]
    public class ReactiveVariableAttribute : LocationInterceptionAspect
    {
        PropertyInfo property;

        /// <inheritdoc />
        public sealed override void OnGetValue(LocationInterceptionArgs args)
        {
            ReactiveManager.WasRead(new CompositeReactiveObject(args.Instance, property));
            args.ProceedGetValue();
        }

        /// <inheritdoc />
        public sealed override void OnSetValue(LocationInterceptionArgs args)
        {
            args.ProceedSetValue();
            ReactiveManager.WasChanged(new CompositeReactiveObject(args.Instance, property));
        }

        public override void RuntimeInitialize(LocationInfo locationInfo)
        {
            property = locationInfo.PropertyInfo;
            base.RuntimeInitialize(locationInfo);
        }
    }
}