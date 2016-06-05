using System;
using System.Collections.Generic;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using SmartReactives.Core;

namespace SmartReactives.PostSharp.NotifyPropertyChanged
{
    [Serializable]
    [IntroduceInterface(typeof(IWeakListener), OverrideAction = InterfaceOverrideAction.Ignore)]
    public class IntroduceWeakListener : InstanceLevelAspect, IWeakListener
    {
#pragma warning disable CS0649
        [ImportMember("OnPropertyChanged", IsRequired = true)]
        public Action<string> onPropertyChanged;
#pragma warning restore CS0649

        public void Notify(object strongKey)
        {
            onPropertyChanged(((PropertyInfo)strongKey).Name);
        }

        static readonly ISet<Type> visited = new HashSet<Type>();
        public static IEnumerable<AspectInstance> IntroduceWeakListenerForType(Type type)
        {
            if (visited.Add(type))
            {
                yield return new AspectInstance(type, new IntroduceWeakListener());
            }
        }

        protected bool Equals(IntroduceWeakListener other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IntroduceWeakListener) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(IntroduceWeakListener left, IntroduceWeakListener right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IntroduceWeakListener left, IntroduceWeakListener right)
        {
            return !Equals(left, right);
        }
    }
}