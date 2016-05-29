using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using SmartReactives.Core;
using SmartReactives.Extensions;
using SmartReactives.List;

namespace SmartReactives.Test
{
	public class ReactiveManagerTest
	{
		public static bool RecoversAfterMissedChange = true;

	    [Test]
	    public void TestDebugMethodAndProperties()
	    {
	        var dependency = new Dependency(0, null);
	        Assert.AreEqual("empty reference", dependency.ToString());

            var variable = new ReactiveVariable<bool>();

            var expression = new ReactiveExpression<bool>(() => variable.Value);
	        expression.Evaluate();
            Assert.AreEqual("unnamed", expression.ToString());
            Assert.True(variable.Dependents.Any());
            Assert.False(expression.Dependents.Any());


            var cache = new ReactiveCache<bool>(() => false);
            Assert.False(cache.Dependents.Any());
        }

        [Test]
        public void TestDisenabler()
        {
            var source = new object();
            var expression = new ReactiveExpression<bool>(() =>
            {
                ReactiveManager.WasRead(source);
                return true;
            });
            int notifications = 0;
            int expectation = 1;
            using (new ReactiveDisenabler())
            {
                expression.Subscribe(getValue => Const(getValue, () => notifications++));
            }
            expression.Evaluate();

            ReactiveManager.WasChanged(source);
            Assert.AreEqual(++expectation, notifications);
            using (new ReactiveDisenabler())
            {
                ReactiveManager.WasChanged(source);
                Assert.AreEqual(expectation, notifications);
                using (new ReactiveDisenabler(true))
                {
                    ReactiveManager.WasChanged(source);
                    Assert.AreEqual(++expectation, notifications);
                }

                ReactiveManager.WasChanged(source);
                Assert.AreEqual(expectation, notifications);
            }
            ReactiveManager.WasChanged(source);
            Assert.AreEqual(++expectation, notifications);
        }

        /// <summary>
        /// Tests whether the system still works after a single ReactiveManager.WasChanged has been 'forgotten' by the user.
        /// </summary>
        [Test]
		public void TestRecoveryMode()
		{
			var expectation = 1;
			var source1 = new DebugReactiveVariable<int>(0, "source1");
			var source2 = new DebugReactiveVariable<int>(0, "source2");
			var selectedSource = source1;
			source1.Value = 0;
			var function = new ReactiveExpression<int>(() => selectedSource.Value);
			var counter = 0;
            function.Subscribe(getValue => Const(getValue, () => counter++));
			source1.Value++;
			expectation++;
			Assert.AreEqual(expectation, counter);
			selectedSource = source2;
			Assert.AreEqual(expectation, counter);

            function.Evaluate();
            source2.Value++;

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
			var function = new ReactiveExpression<bool>(() =>
			{
				ReactiveManager.WasRead(source1);
				return true;
			});
			var counter = 0;
			var expectation = 1;
			function.Subscribe(getValue => Const(getValue, () => counter++));

			ReactiveManager.WasChanged(source1);
			Assert.AreEqual(++expectation, counter);

			ReactiveManager.WasChanged(source2);
			Assert.AreEqual(expectation, counter);

			ReactiveManager.WasChanged(source3);
			Assert.AreEqual(expectation, counter);
		}

		/// <summary>
		/// Asserts that <see cref="ConditionalWeakTable{TKey,TValue}" /> uses reference equality.
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
			var source = new DebugReactiveVariable<bool>(false, "source");
			source.Value = true;
			var sink = new ReactiveExpression<bool>(() => source.Value && source.Value, "sink");

			var counter = 0;
		    sink.Subscribe(getSink => Const(getSink, () => counter++));

            Assert.AreEqual(1, counter);
            
			source.Value = false;
			Assert.AreEqual(2, counter);
		}

        public static U Const<T,U>(Func<T> getFirstValue, Func<U> getSecondValue)
        {
            getFirstValue();
            return getSecondValue();
        }

		[Test]
		public void TestIndirectDiamondSituation()
		{
			var source = new DebugReactiveVariable<bool>(false ,"source");
			source.Value = true;
			var mid1 = new ReactiveExpression<bool>(() => source.Value, "mid1");
			var mid2 = new ReactiveExpression<bool>(() => source.Value, "mid2");
			var sink = new ReactiveExpression<bool>(() => mid1.Evaluate() && mid2.Evaluate(), "sink");
			var counter = 0;
            sink.Subscribe(getSink => Const(getSink, () => counter++));
            
			Assert.AreEqual(1, counter);
			source.Value = false;
			Assert.AreEqual(2, counter);
        }

        [Test]
        public void TestIndirectDiamondSituation2()
        {
            var input = new ReactiveVariable<int>();
            var timesTwo = new ReactiveExpression<int>(() => input.Value * 2, "timesTwo");
            var plusOne = new ReactiveExpression<int>(() => input.Value + 1, "plusOne");
            var sumOfBoth = new ReactiveExpression<int>(() => timesTwo.Evaluate() + plusOne.Evaluate(), "sumOfBoth");
            var counter = 0;
            sumOfBoth.Subscribe(getValue => Const(getValue, () => counter++));
            Assert.AreEqual(1, counter);
            input.Value = 1; 
            Assert.AreEqual(2, counter);
            input.Value = 2;
            Assert.AreEqual(3, counter);
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

		class EqualityBasedOnId
		{
			readonly int id;

			public EqualityBasedOnId(int id)
			{
				this.id = id;
			}

			protected bool Equals(EqualityBasedOnId other)
			{
				return id == other.id;
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
				return Equals((EqualityBasedOnId) obj);
			}

			public override int GetHashCode()
			{
				return id;
			}
		}
	}
}