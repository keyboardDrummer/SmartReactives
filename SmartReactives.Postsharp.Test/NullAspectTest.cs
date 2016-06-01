using System;
using NUnit.Framework;
using SmartReactives.PostSharp.NotifyPropertyChanged;

// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Local

namespace SmartReactives.Postsharp.Test
{
    /// <summary>
    /// In these scenarios, a superclass will access a base class's properties,
    /// while the superclass is constructor and before the baseclass is constructing.
    /// This is troublesome for PostSharp, because it initializes instance aspects for the base class at the start of the base class constructor.
    /// </summary>
    public class NullAspectTest
    {
        readonly bool AllowParentClassInspectionOfBaseClassPropertiesDuringConstruction = true;

        [Test]
        public void TestPropertyAccessDuringConstructionReflection()
        {
            if (AllowParentClassInspectionOfBaseClassPropertiesDuringConstruction)
            {
                var value = new HasReactiveVariablePropertyAccessedByParentInParentConstructor();
                Assert.NotNull(value);
            }
            else
            {
                Assert.Throws<Exception>(() =>
                {
                    var value = new HasReactiveVariablePropertyAccessedByParentInParentConstructor();
                    Assert.NotNull(value);
                });
            }
        }

        [Test]
        public void TestFunctionPropertyAccessDuringConstructionReflection()
        {
            if (AllowParentClassInspectionOfBaseClassPropertiesDuringConstruction)
            {
                var value = new HasReactiveFunctionPropertyAccessedByParentInParentConstructor();
                Assert.NotNull(value);
            }
            else
            {
                Assert.Throws<Exception>(() =>
                {
                    var value = new HasReactiveFunctionPropertyAccessedByParentInParentConstructor();
                    Assert.NotNull(value);
                });
            }
        }

        [Test]
        public void TestPropertyAccessDuringConstruction()
        {
            var value = new PropertyAccessDuringConstruction();
            Assert.NotNull(value);
        }

        [Test]
        public void TestAccessPropertyInBaseClassConstructor()
        {
            new CallsPropertyInOverridenMethod();
        }

        abstract class UsesReflectionToAccessAllPropertiesDuringConstruction : HasNotifyPropertyChanged
        {
            protected UsesReflectionToAccessAllPropertiesDuringConstruction()
            {
                foreach (var property in GetType().GetProperties())
                {
                    var value = property.GetValue(this);
                    if (property.CanWrite)
                    {
                        property.SetValue(this, value);
                    }
                }
            }
        }

        class HasReactiveVariablePropertyAccessedByParentInParentConstructor : UsesReflectionToAccessAllPropertiesDuringConstruction
        {
            [SmartNotifyPropertyChanged]
            public bool Woop { get; set; }
        }

        class HasReactiveFunctionPropertyAccessedByParentInParentConstructor : UsesReflectionToAccessAllPropertiesDuringConstruction
        {
            [SmartNotifyPropertyChanged]
            public bool Woop
            {
                get { return true; }
            }
        }

        class PropertyAccessDuringConstruction : HasNotifyPropertyChanged
        {
            public PropertyAccessDuringConstruction()
            {
                Console.Write(Woop);
            }

            [SmartNotifyPropertyChanged]
            public bool Woop { get; set; }
        }

        class CallsVirtualMethodInConstructor : HasNotifyPropertyChanged
        {
            public CallsVirtualMethodInConstructor()
            {
                AllowOverride();
            }

            protected virtual void AllowOverride()
            {
            }
        }

        class CallsPropertyInOverridenMethod : CallsVirtualMethodInConstructor
        {
            [SmartNotifyPropertyChanged]
            public object MyProperty { get; set; }

            protected override void AllowOverride()
            {
                base.AllowOverride();
                Console.Write(MyProperty);
            }
        }
    }
}