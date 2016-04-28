using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using SmartReactives.Core;
using SmartReactives.Extensions;
using SmartReactives.List;

namespace SmartReactives.Test.Reactive
{
	public class ReactiveManagerTest
	{
		public static bool RecoversAfterMissedChange = true;
		private class EqualityBasedOnId
		{
			private readonly int _id;

			public EqualityBasedOnId(int id)
			{
				_id = id;
			}

			protected bool Equals(EqualityBasedOnId other)
			{
				return _id == other._id;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
				{
					return false;
				}
				if (ReferenceEquals(this, obj))
				{
					return true;
				}
				if (obj.GetType() != GetType())
				{
					return false;
				}
				return Equals((EqualityBasedOnId)obj);
			}

			public override int GetHashCode()
			{
				return _id;
			}
		}

		/// <summary>
		/// Tests whether the system still works after a single ReactiveManager.WasChanged has been 'forgotten' by the user.
		/// </summary>
		[Test]
		public void TestRecoveryMode()
		{
			var expectation = 0;
			var source1 = new DebugReactiveVariable<int>("source1");
			var source2 = new DebugReactiveVariable<int>("source2");
			var selectedSource = source1;
			source1.Value = 0;
			var function = new ObservableExpression<int>(() => selectedSource.Value);
			var counter = 0;
			function.Subscribe(_ => counter++);
			Assert.AreEqual(0, function.Evaluate());
			source1.Value++;
			expectation++;
			Assert.AreEqual(expectation, counter);
			Assert.AreEqual(1, function.Evaluate());
			selectedSource = source2;
			Assert.AreEqual(expectation, counter);
			Assert.AreEqual(0, function.Evaluate());
			source2.Value++;
			Assert.AreEqual(1, function.Evaluate());
			if (RecoversAfterMissedChange)
			{
				expectation++;
			}
			Assert.AreEqual(expectation, counter);
		}

		/// <summary>
		/// Tests how ReactiveManager does its comparisons, by reference or by semantic equality.
		/// Asserts that ReactiveManager compares sources by reference.
		/// </summary>
		[Test]
		public void TestSourceEquality()
		{
			var source1 = new EqualityBasedOnId(1);
			var source2 = new EqualityBasedOnId(2);
			var source3 = new EqualityBasedOnId(1);
			var function = new ObservableExpression<bool>(() =>
			{
				ReactiveManager.WasRead(source1);
				return true;
			});
			var counter = 0;
			var expectation = 0;
			function.Subscribe(_ => counter++);
			function.Evaluate();

			ReactiveManager.WasChanged(source1);
			Assert.AreEqual(++expectation, counter);

			ReactiveManager.WasChanged(source2);
			Assert.AreEqual(expectation, counter);

			ReactiveManager.WasChanged(source3);
			Assert.AreEqual(expectation, counter);
		}

		/// <summary>
		/// Asserts that <see cref="ConditionalWeakTable{TKey,TValue}"/> uses reference equality.
		/// </summary>
		[Test]
		public void TestConditionalWeakTableEquality()
		{
			var table = new ConditionalWeakTable<EqualityBasedOnId, object>();
			var source1 = new EqualityBasedOnId(1);
			var source2 = new EqualityBasedOnId(1);
			var value1 = new object();
			table.Add(source1, value1);
			Assert.AreNotEqual(value1, table.GetOrCreateValue(source2));
		}

		[Test]
		public void TestNullCannotBeASource()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				ReactiveManager.Evaluate(new Dependent(), () =>
				{
					ReactiveManager.WasRead(null);
					return true;
				});
			});
		}

		[Test]
		public void TestNullList()
		{
			ReactiveManagerWithList.Evaluate<INotifyCollectionChanged>(() => null);
		}

		[Test]
		public void TestDirectDiamondSituation()
		{
			var source = new DebugReactiveVariable<bool>("source");
			source.Value = true;
			var sink = new ObservableExpression<bool>(() => source.Value && source.Value, "sink");

			var counter = 0;
			sink.Subscribe(_ => counter++);

			Assert.AreEqual(true, sink.Evaluate());
			Assert.AreEqual(0, counter);
			source.Value = false;
			Assert.AreEqual(1, counter);
		}

		[Test]
		public void TestIndirectDiamondSituation()
		{
			var source = new DebugReactiveVariable<bool>("source");
			source.Value = true;
			var mid1 = new ObservableExpression<bool>(() => source.Value, "mid1");
			var mid2 = new ObservableExpression<bool>(() => source.Value, "mid2");
			var sink = new ObservableExpression<bool>(() => mid1.Evaluate() && mid2.Evaluate(), "sink");
			var counter = 0;
			sink.Subscribe(_ => counter++);

			Assert.AreEqual(true, sink.Evaluate());
			Assert.AreEqual(0, counter);
			source.Value = false;
			Assert.AreEqual(1, counter);
		}

		[Test]
		public void DependenciesClearedAfterNotify()
		{
			var notifyCounter = new NotifyCounter();
			var source = new object();
			ReactiveManager.Evaluate(notifyCounter, () =>
			{
				ReactiveManager.WasRead(source);
				return true;
			});
			Assert.AreEqual(0, notifyCounter.Counter);
			ReactiveManager.WasChanged(source);
			Assert.AreEqual(1, notifyCounter.Counter);
			ReactiveManager.WasChanged(source);
			Assert.AreEqual(1, notifyCounter.Counter);
		}
	}
}
