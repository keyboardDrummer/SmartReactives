using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NUnit.Framework;
using SmartReactives.Core;
using SmartReactives.Extensions;
using SmartReactives.Postsharp.Test;

// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace SmartReactives.Test
{
	public class ReactiveVariableTest
	{
		const int NotificationWhenDependentChangesToSameValue = 1; //Ideally 0
		const int NotificationWhenSourceChangesToSameValue = 0; //Ideally 0
		const int WorksWithoutInitialPropertyAccess = 0; //Ideally 1
		const int RecordsChangingDependenciesWithoutAccess = 0;
        

        [Test]
		public void ReactiveVariable()
		{
			var test = new Source();
			var rx = new ReactiveExpression<string>(() => test.Woop ? "foo" : "bar");
			var counter = 0;
			var expectation = 1;
			rx.Subscribe(s => ReactiveManagerTest.Const(s, () => counter++));
			test.Woop = true;
			Assert.AreEqual(++expectation, counter);
			test.Woop = true;
			Assert.AreEqual(expectation += NotificationWhenSourceChangesToSameValue, counter);
			test.Woop = false;
			Assert.AreEqual(++expectation, counter);
		}

		[Test]
		public void ComplexExpression()
		{
			var first = new Source();
			var second = new Source();
			var rx = new ReactiveExpression<string>(() => first.Woop ? "foo" : (second.Woop ? "bar" : "zoo"), "rx");
			var counter = 0;
		    var expectation = 1;
			Assert.AreEqual("zoo", rx.Evaluate());
			rx.Subscribe(s => counter++);
			second.Woop = true;
			Assert.AreEqual(++expectation, counter);
			Assert.AreEqual("bar", rx.Evaluate());
			first.Woop = true;
			Assert.AreEqual("foo", rx.Evaluate());
			Assert.AreEqual(++expectation, counter);
			second.Woop = false;
			Assert.AreEqual("foo", rx.Evaluate());
			Assert.AreEqual(expectation, counter);
			second.Woop = true;
			Assert.AreEqual("foo", rx.Evaluate());
			Assert.AreEqual(expectation, counter);
			first.Woop = false;
			Assert.AreEqual("bar", rx.Evaluate());
			Assert.AreEqual(++expectation, counter);
			second.Woop = false;
			Assert.AreEqual("zoo", rx.Evaluate());
			Assert.AreEqual(++expectation, counter);
		}

		[Test]
		public void ActiveAfterUsage()
		{
			var source = new Source();
			var user = new SimpleSink(source);
			var counter = 0;
		    var expectation = 0;
			user.BoopReactive.Skip(1).Subscribe(getValue => counter++);

			Assert.AreEqual(expectation, counter);
			source.FlipWoop();
			Assert.AreEqual(expectation, counter);

			Assert.AreEqual(user.Boop, user.Boop);
			source.FlipWoop();
			Assert.AreEqual(++expectation, counter);
		}

		[Test]
		public void Reactive()
		{
			var source1 = new Source();
			var source2 = new Source();
			var user = new SimpleSink(source1);
			var counter = 0;
		    var expectation = 1;
            user.BoopReactive.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));

			Assert.AreEqual(expectation, counter);
			Assert.AreEqual(false, user.Boop);

			source1.Woop = true;
			Assert.AreEqual(++expectation, counter);
			Assert.AreEqual(true, user.Boop);

			source2.Woop = true;
			Assert.AreEqual(expectation, counter);
			Assert.AreEqual(true, user.Boop);

			source1.Woop = false;
			Assert.AreEqual(++expectation, counter);
			Assert.AreEqual(false, user.Boop);

			user.Source = source2;
			Assert.AreEqual(++expectation, counter);
			Assert.AreEqual(true, user.Boop);

			source2.Woop = false;
			Assert.AreEqual(++expectation, counter);
			Assert.AreEqual(false, user.Boop);
		}

		[Test]
		public void TestChangingDependenciesForSinkTrap()
		{
			var first = new Source();
			first.Woop = true;
			var second = new Source();
			var sink = new ComplexSink(first, second);
			var counter = 0;
			var expectation = 1;
			Assert.AreEqual("foo", sink.Complex);
			first.Woop = false;
			new ReactiveExpression<string>(() => sink.Complex).Subscribe(value => counter++);
			Assert.AreEqual(expectation, counter);
			second.Woop = true;
			Assert.AreEqual(expectation += RecordsChangingDependenciesWithoutAccess, counter);
		}

		[Test]
		public void TestEventWithoutPropertyAccess()
		{
			var source = new Source();
			var sink = new SimpleSink(source);

			var counter = 0;
			var expectation = 1;
            new ReactiveExpression<bool>(() => sink.Boop).Subscribe(value => counter++);
			Assert.AreEqual(expectation, counter);
			source.FlipWoop();
			Assert.AreEqual(expectation += WorksWithoutInitialPropertyAccess, counter);
		}

		[Test]
		public void TestComplexSink()
		{
			var first = new Source();
			var second = new Source();
			var sink = new ComplexSink(first, second);
			var counter = 0;
			var expectation = 1;
			Assert.AreEqual("zoo", sink.Complex);
            new ReactiveExpression<string>(() => sink.Complex).Subscribe(value => ReactiveManagerTest.Const(value, () => counter++));
			Assert.AreEqual(expectation, counter);
			second.Woop = true;
			Assert.AreEqual(++expectation, counter);
			Assert.AreEqual("bar", sink.Complex);
			first.Woop = true;
			Assert.AreEqual("foo", sink.Complex);
			Assert.AreEqual(++expectation, counter);
			second.Woop = false;
			Assert.AreEqual("foo", sink.Complex);
			Assert.AreEqual(expectation, counter);
			second.Woop = true;
			Assert.AreEqual("foo", sink.Complex);
			Assert.AreEqual(expectation, counter);
			first.Woop = false;
			Assert.AreEqual("bar", sink.Complex);
			Assert.AreEqual(++expectation, counter);
			second.Woop = false;
			Assert.AreEqual("zoo", sink.Complex);
			Assert.AreEqual(++expectation, counter);

			sink.Source2 = new Source();
			Assert.AreEqual("zoo", sink.Complex);
			Assert.AreEqual(expectation += NotificationWhenDependentChangesToSameValue, counter);
		}

		[Test]
		public void TestSinkChain()
		{
			var sink = new SinkChain();
			var outerCounter = 0;
			Assert.AreEqual(false, sink.OuterSink);
			Assert.AreEqual(true, sink.InnerSink);
            sink.OuterSinkReactive.Skip(1).Subscribe(value => ReactiveManagerTest.Const(value, () => outerCounter++));

            Assert.AreEqual(0, outerCounter);
            sink.Source = true;
			Assert.AreEqual(true, sink.OuterSink);
			Assert.AreEqual(false, sink.InnerSink);
			Assert.AreEqual(1, outerCounter);
		}

		[Test]
		public void MultipleDependence()
		{
			var source = new Source();
			var sink = new ReactiveExpression<bool>(() => source.Woop && source.Woop);
			var counter = 0;
		    var expectation = 1;
			sink.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => counter++));
			Assert.AreEqual(source.Woop, sink.Evaluate());
			Assert.AreEqual(expectation, counter);
			source.FlipWoop();
			Assert.AreEqual(++expectation, counter);
		}

		[Test]
		public void NestedMultipleDependence()
		{
			var source1 = new Source();
			var source2 = new Source();
			var mid = new ReactiveExpression<bool>(() => source1.Woop ^ source2.Woop, "mid");
			var top = new ReactiveExpression<bool>(() => mid.Evaluate() & source1.Woop, "top");
			var topCounter = 0;
			var midCounter = 0;
			var topExpectation = 1;
			var midExpectation = 1;

			top.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => topCounter++));
            mid.Subscribe(getValue => ReactiveManagerTest.Const(getValue, () => midCounter++));
			Assert.AreEqual(topExpectation, topCounter);
			Assert.AreEqual(midExpectation, midCounter);
			source1.FlipWoop();
			Assert.AreEqual(++topExpectation, topCounter);
			Assert.AreEqual(++midExpectation, midCounter);

			source2.FlipWoop();
			Assert.AreEqual(++topExpectation, topCounter);
			Assert.AreEqual(++midExpectation, midCounter);
		}

		[Test]
		public void TestTree()
		{
			var bottom1 = new Tree();
			var bottom2 = new Tree();
			var bottoms1 = new List<Tree> {bottom1, bottom2};
			var mid1 = new Tree(bottoms1);
			var mid2 = new Tree();
			var mids = new List<Tree> {mid1, mid2};
			var top = new Tree(mids);

			var counter = 0;

            top.HasSuccessfulBloodReactive.Subscribe(value => ReactiveManagerTest.Const(value, () => counter++));
			var expectation = 1;

			mid2.Successful = true;
			Assert.AreEqual(++expectation, counter);
			mid1.Successful = true;
			Assert.AreEqual(++expectation, counter);
			mid2.Successful = false;
			Assert.AreEqual(expectation, counter);

			Assert.AreEqual(true, top.HasSuccessfulBlood);
			mid1.Successful = false;
			Assert.AreEqual(++expectation, counter);

			Assert.AreEqual(false, top.HasSuccessfulBlood);
			bottom1.Successful = true;
			Assert.AreEqual(++expectation, counter);

			Assert.AreEqual(true, top.HasSuccessfulBlood);
			bottom2.Successful = true;
			Assert.AreEqual(expectation, counter);
		}
	}
}