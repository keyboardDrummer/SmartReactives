﻿using System;
using NUnit.Framework;
using SmartReactives.Common;
using SmartReactives.PostSharp;
using SmartReactives.PostSharp.NotifyPropertyChanged;
using SmartReactives.Test;

// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace SmartReactives.Postsharp.Test
{
    public class ReactiveVariableAttributeTest
    {
        class HasReactiveVariableAttribute
        {
            [ReactiveVariable]
            public bool Woop { get; set; }
        }

        [Test]
        public void TestReactiveVariable()
        {
            var woop = new HasReactiveVariableAttribute();
            var expression = new ReactiveExpression<bool>(() => woop.Woop);
            int counter = 0;
            var expectation = 1;
            expression.Subscribe(get => ReactiveManagerTest.Const(get, () => counter++));
            woop.Woop = !woop.Woop;
            Assert.AreEqual(++expectation, counter);
        }

        [Test]
        public void ForceSink()
        {
            var source = new Source();
            var badSetter = new BadSetterFixed(source);
            var counter = 0;
            badSetter.PropertyChanged += (sender, args) => counter++;
            Assert.AreEqual(source.Woop, badSetter.Bad);
            Assert.AreEqual(0, counter);
            source.FlipWoop();
            Assert.AreEqual(1, counter);
        }

        class BadSetterFixed : HasNotifyPropertyChanged
        {
            readonly Source source;

            public BadSetterFixed(Source source)
            {
                this.source = source;
            }

            [SmartNotifyPropertyChanged]
            public bool Bad
            {
                get { return source.Woop; }
                set { source.Woop = value; }
            }
        }
    }
}