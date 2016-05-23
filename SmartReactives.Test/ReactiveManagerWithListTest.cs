using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using NUnit.Framework;
using SmartReactives.Extensions;
using SmartReactives.Postsharp.NotifyPropertyChanged;
using SmartReactives.Test.Postsharp;

namespace SmartReactives.Test
{
	public class ReactiveManagerWithListTest
	{
		class ClassWithList : HasNotifyPropertyChanged
		{
			[ReactiveList]
			[SmartNotifyPropertyChanged]
			public ObservableCollection<Source> Sources { get; } = new ObservableCollection<Source>();
		}

		[Test]
		public void TestList()
		{
			var source1 = new Source();
			var source2 = new Source();
			var source3 = new Source();
			var sourcesList = new ClassWithList();
			sourcesList.Sources.Add(source1);
			sourcesList.Sources.Add(source2);

			int counter = 0;
			int expectation = 1;
			var property = new ReactiveExpression<bool>(() => sourcesList.Sources[1].Woop);
			property.Evaluate();
		    property.Subscribe(s => ReactiveManagerTest.Const(s, () => counter++));

			source1.Woop = true;
			Assert.AreEqual(expectation, counter);

			source2.Woop = true;
			Assert.AreEqual(true, property.Evaluate());
			Assert.AreEqual(++expectation, counter);

			source1.Woop = false;
			sourcesList.Sources.Insert(0, source3);
			Assert.AreEqual(false, property.Evaluate());
			Assert.AreEqual(++expectation, counter);

			source2.Woop = false;
			source3.Woop = true;
			Assert.AreEqual(expectation, counter);
			source1.Woop = true;
			Assert.AreEqual(true, property.Evaluate());
			Assert.AreEqual(++expectation, counter);
		}

		[Test]
		public void TestList2()
		{
			var sourcesList = new ClassWithList();
			var property = new ReactiveExpression<int>(() => sourcesList.Sources.Count);
			property.Evaluate();
			int counter = 0;
			int expectation = 1;
			property.Subscribe(s => ReactiveManagerTest.Const(s, () => counter++));

			Assert.AreEqual(expectation, counter);
			var source1 = new Source();
			sourcesList.Sources.Add(source1);
			Assert.AreEqual(++expectation, counter);

			Assert.AreEqual(1, property.Evaluate());
			var source2 = new Source();
			sourcesList.Sources.Add(source2);
			Assert.AreEqual(++expectation, counter);

			source1.Woop = true;
			Assert.AreEqual(expectation, counter);
		}

		class DependentList : HasNotifyPropertyChanged
		{
			[ReactiveList]
			[SmartNotifyPropertyChanged]
			public ObservableCollection<Source> Dependent => Sources;

			[SmartNotifyPropertyChanged]
			public int DependentCount => Dependent.Count;

			[ReactiveList]
			[SmartNotifyPropertyChanged]
			public ObservableCollection<Source> Sources { get; set; } = new ObservableCollection<Source>();

			[SmartNotifyPropertyChanged]
			public int SourcesCount => Sources.Count;
		}

		[Test]
		public void TestDependentList()
		{
			var obj = new DependentList();
			var dependentCounter = 0;
			var sourcesCounter = 0;
			var dependentExpectation = 0;
			var sourcesExpectation = 0;
			ObservableUtility.FromProperty(() => obj.DependentCount).Subscribe(_ => dependentCounter++);
			ObservableUtility.FromProperty(() => obj.Sources).Subscribe(_ => sourcesCounter++);

			Assert.AreEqual(++sourcesExpectation, sourcesCounter);
			Assert.AreEqual(++dependentExpectation, dependentCounter);
			Assert.AreEqual(0, obj.DependentCount);
			Assert.AreEqual(0, obj.SourcesCount);
			obj.Sources = new ObservableCollection<Source>();
			Assert.AreEqual(++sourcesExpectation, sourcesCounter);
			Assert.AreEqual(++dependentExpectation, dependentCounter);

			Assert.AreEqual(0, obj.DependentCount);
			Assert.AreEqual(0, obj.SourcesCount);
			obj.Sources.Add(new Source());
			Assert.AreEqual(1, obj.DependentCount);
			Assert.AreEqual(1, obj.SourcesCount);
			Assert.AreEqual(++sourcesExpectation, sourcesCounter);
			Assert.AreEqual(++dependentExpectation, dependentCounter);

			obj.Dependent.Add(new Source());
			Assert.AreEqual(++dependentExpectation, dependentCounter);

			Assert.AreEqual(2, obj.DependentCount);
			Assert.AreEqual(2, obj.SourcesCount);
			var newList = new ObservableCollection<Source>();
			obj.Sources = newList;
			Assert.AreEqual(0, obj.DependentCount);
			Assert.AreEqual(0, obj.SourcesCount);
			Assert.AreEqual(++dependentExpectation, dependentCounter);

			obj.Sources.Add(new Source());
			Assert.AreEqual(++dependentExpectation, dependentCounter);

			Assert.AreEqual(1, obj.DependentCount);
			Assert.AreEqual(1, obj.SourcesCount);
			obj.Sources.RemoveAt(0);
			Assert.AreEqual(++dependentExpectation, dependentCounter);
		}

		/// <summary>
		/// While the victim is being Notified, the attacker adds a new source to its list.
		/// </summary>
		[Test]
		public void DependentListMultiThreadingTrap()
		{
			var list = new DependentList();
			var counter = 0;
			var victimWaiter = new Waiter();
			var attackerWaiter = new Waiter();

            var observableExpression = new ReactiveExpression<int>(() => list.DependentCount, "DependentCountObserver");
		    observableExpression.Evaluate();

            var victim = new Thread(() =>
            {
                observableExpression.Skip(1).Subscribe(s =>
                {
                    s();
                    counter++;
                    attackerWaiter.Release();
                    victimWaiter.Wait();
                });

                list.Sources.Add(new Source());
                Assert.AreEqual(1, counter);
            });

			var attackerList = new DependentList();

			var attacker = new Thread(() =>
			{
				attackerWaiter.Wait();
				attackerList.Sources.Add(new Source());
				victimWaiter.Release();
			});
            
			Assert.AreEqual(0, attackerList.DependentCount);

			victim.Start();
			attacker.Start();

			victim.Join();
			attacker.Join();

			Assert.AreEqual(1, counter);
		}
	}
}
