using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmartReactives.Common;
using SmartReactives.Core;

namespace SmartReactives.Test
{
    public class WeakStrongListenerTest
    {
        class Listener : IWeakListener
        {
            readonly ISubject<object> notifications = new Subject<object>();

            public IObservable<object> Notifications => notifications;

            public void Notify(object strongKey)
            {
                notifications.OnNext(strongKey);
            }
        }

        [Test]
        public void TestWeakStrongExpression()
        {
            var listener = new Listener();
            var source = new ReactiveVariable<int>(3);
            var listenerValue = ReactiveManager.Evaluate(new CompositeReactiveObject(listener, "first"), () => source.Value);
            Assert.AreEqual(source.Value, listenerValue);
            var results = new List<object>();
            listener.Notifications.Subscribe(result => results.Add(result));
            source.Value++;
            Assert.AreEqual("first", results[0]);
        }

        [Test]
        public void TestWeakStrongVariable()
        {
            var listener = new Listener();
            var source = new CompositeReactiveObject(listener, "first");
            var sameSource = new CompositeReactiveObject(listener, "first");
            var otherSource = new CompositeReactiveObject(listener, "second");
            var expression = Reactive.Expression(() =>
            {
                ReactiveManager.WasRead(source);
                return 1;
            });
            var listenerValue = expression.Evaluate();
            Assert.AreEqual(1, listenerValue);

            var counter = 0;
            var expectation = 0;
            expression.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
            Assert.AreEqual(++expectation, counter);
            ReactiveManager.WasChanged(sameSource);
            Assert.AreEqual(++expectation, counter);

            ReactiveManager.WasChanged(otherSource);
            Assert.AreEqual(expectation, counter);
        }
    }
}
