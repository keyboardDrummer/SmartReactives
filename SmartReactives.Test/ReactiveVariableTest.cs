using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NUnit.Framework;
using SmartReactives.Extensions;
using SmartReactives.Postsharp.NotifyPropertyChanged;
using SmartReactives.Test.Postsharp;

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
			var expectation = 0;
			Assert.AreEqual("bar", rx.Evaluate());
			rx.Subscribe(s => counter++);
			test.Woop = true;
			Assert.AreEqual("foo", rx.Evaluate());
			Assert.AreEqual(++expectation, counter);
			test.Woop = true;
			Assert.AreEqual(expectation += NotificationWhenSourceChangesToSameValue, counter);
			test.Woop = false;
			Assert.AreEqual(++expectation, counter);
			Assert.AreEqual("bar", rx.Evaluate());
		}

		[Test]
		public void ComplexExpression()
		{
			var first = new Source();
			var second = new Source();
			var rx = new ReactiveExpression<string>(() => first.Woop ? "foo" : (second.Woop ? "bar" : "zoo"), "rx");
			var counter = 0;
			Assert.AreEqual("zoo", rx.Evaluate());
			rx.Subscribe(s => counter++);
			second.Woop = true;
			Assert.AreEqual(1, counter);
			Assert.AreEqual("bar", rx.Evaluate());
			first.Woop = true;
			Assert.AreEqual("foo", rx.Evaluate());
			Assert.AreEqual(2, counter);
			second.Woop = false;
			Assert.AreEqual("foo", rx.Evaluate());
			Assert.AreEqual(2, counter);
			second.Woop = true;
			Assert.AreEqual("foo", rx.Evaluate());
			Assert.AreEqual(2, counter);
			first.Woop = false;
			Assert.AreEqual("bar", rx.Evaluate());
			Assert.AreEqual(3, counter);
			second.Woop = false;
			Assert.AreEqual("zoo", rx.Evaluate());
			Assert.AreEqual(4, counter);
		}

		[Test]
		public void ActiveAfterUsage()
		{
			var source = new Source();
			var user = new SimpleSink(source);
			var counter = 0;
			user.PropertyChanged += (sender, args) => counter++;

			Assert.AreEqual(0, counter);
			source.FlipWoop();
			Assert.AreEqual(0, counter);

			Assert.AreEqual(user.Boop, user.Boop);
			source.FlipWoop();
			Assert.AreEqual(1, counter);
		}

		[Test]
		public void Reactive()
		{
			var source1 = new Source();
			var source2 = new Source();
			var user = new SimpleSink(source1);
			var counter = 0;
			ObservableUtility.FromProperty(() => user.Boop).Skip(1).Subscribe(value => counter++);

			Assert.AreEqual(0, counter);
			Assert.AreEqual(false, user.Boop);

			source1.Woop = true;
			Assert.AreEqual(1, counter);
			Assert.AreEqual(true, user.Boop);

			source2.Woop = true;
			Assert.AreEqual(1, counter);
			Assert.AreEqual(true, user.Boop);

			source1.Woop = false;
			Assert.AreEqual(2, counter);
			Assert.AreEqual(false, user.Boop);

			user.Source = source2;
			Assert.AreEqual(3, counter);
			Assert.AreEqual(true, user.Boop);

			source2.Woop = false;
			Assert.AreEqual(4, counter);
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
			var expectation = 0;
			Assert.AreEqual("foo", sink.Complex);
			first.Woop = false;
			ObservableUtility.FromProperty(() => sink.Complex, false).Subscribe(value => { counter++; });
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
			var expectation = 0;
			ObservableUtility.FromProperty(() => sink.Boop, false).Subscribe(value => { counter++; });
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
			var expectation = 0;
			Assert.AreEqual("zoo", sink.Complex);
			ObservableUtility.FromProperty(() => sink.Complex).Skip(1).Subscribe(value => { counter++; });
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
			ObservableUtility.FromProperty(() => sink.OuterSink).Skip(1).Subscribe(value => outerCounter++);

			sink.Source = true;
			Assert.AreEqual(true, sink.OuterSink);
			Assert.AreEqual(false, sink.InnerSink);
			Assert.AreEqual(1, outerCounter);
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

		[Test]
		public void MultipleDependence()
		{
			var source = new Source();
			var sink = new ReactiveExpression<bool>(() => source.Woop && source.Woop);
			var counter = 0;

			sink.Subscribe(_ => counter++);
			Assert.AreEqual(source.Woop, sink.Evaluate());
			Assert.AreEqual(0, counter);
			source.FlipWoop();
			Assert.AreEqual(1, counter);
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
			var topExpectation = 0;
			var midExpectation = 0;

			top.Subscribe(_ => { topCounter++; });
			mid.Subscribe(_ => midCounter++);
			Assert.AreEqual(source1.Woop, top.Evaluate());
			Assert.AreEqual(topExpectation, topCounter);
			Assert.AreEqual(midExpectation, midCounter);
			source1.FlipWoop();
			Assert.AreEqual(++topExpectation, topCounter);
			Assert.AreEqual(++midExpectation, midCounter);

			Assert.AreEqual(source1.Woop, top.Evaluate());
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
			ObservableUtility.FromProperty(() => top.HasSuccessfulBlood).Subscribe(_ => counter++);
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
		
		class SimpleSink : HasNotifyPropertyChanged
		{
			public SimpleSink(Source source)
			{
				Source = source;
			}

			[SmartNotifyPropertyChanged]
			public Source Source { get; set; }

			[SmartNotifyPropertyChanged]
			public bool Boop => Source.Woop;
		}

		class ComplexSink : HasNotifyPropertyChanged
		{
			public ComplexSink(Source source, Source source2)
			{
				Source = source;
				Source2 = source2;
			}

			[SmartNotifyPropertyChanged]
			public Source Source { get; }

			[SmartNotifyPropertyChanged]
			public Source Source2 { get; set; }

			[SmartNotifyPropertyChanged]
			public string Complex => Source.Woop ? "foo" : (Source2.Woop ? "bar" : "zoo");
		}

		class SinkChain : HasNotifyPropertyChanged
		{
			[SmartNotifyPropertyChanged]
			public bool Source { get; set; }

			[SmartNotifyPropertyChanged]
			public bool InnerSink => !Source;

			[SmartNotifyPropertyChanged]
			public bool OuterSink => !InnerSink;
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

		class Tree : HasNotifyPropertyChanged
		{
			public Tree(IList<Tree> children = null, bool successful = false)
			{
				Children = children ?? new List<Tree>();
				Successful = successful;
			}

			[SmartNotifyPropertyChanged]
			public IList<Tree> Children { get; }

			[SmartNotifyPropertyChanged]
			public bool Successful { get; set; }

			[SmartNotifyPropertyChanged]
			public bool HasSuccessfulBlood
			{
				get { return Successful || Children.Any(child => child.HasSuccessfulBlood); }
			}
		}
	}
}