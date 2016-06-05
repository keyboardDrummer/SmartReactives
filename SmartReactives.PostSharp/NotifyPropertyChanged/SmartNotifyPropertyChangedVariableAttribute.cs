using System;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using SmartReactives.Core;

namespace SmartReactives.PostSharp.NotifyPropertyChanged
{
    /// <summary>
    /// A variable property is one which uses a backing field, and does not dependend on other properties.
    /// Place this attribute on a variable property to enable dependencies on this property to be tracking automatically.
    /// </summary>
    [ProvideAspectRole("SmartNotifyPropertyChanged")]
    [AttributeUsage(AttributeTargets.Property)]
    [Serializable]
    public class SmartNotifyPropertyChangedVariableAttribute : SmartNotifyPropertyChangedVariableAttributeBase
    {
        public sealed override void OnGetValue(LocationInterceptionArgs args)
        {
            ReactiveManager.WasRead(new WeakStrongReactive(args.Instance, property));
            args.ProceedGetValue();
        }
    }
}