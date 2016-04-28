using System;
using System.Linq;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using PostSharp.Reflection;
using SmartReactives.Core;

namespace SmartReactives.Postsharp.NotifyPropertyChanged
{
	/// <summary>
	/// Makes forwarder properties reactive.
	/// A forwarder property wraps around another property by having both a getter and setter that refer to the underlying property.
	/// </summary>
	[ProvideAspectRole("NotifyPropertyChangedSubAspect")]
	[Serializable]
	[AttributeUsage(AttributeTargets.Property)]
	public class SmartNotifyPropertyChangedAttribute : SmartNotifyPropertyChangedVariableAttributeBase, IListener //TODO improve comment and name.
	{
		protected string _propertyName;
		private readonly Action<string> _raisePropertyChanged;

		public SmartNotifyPropertyChangedAttribute()
		{
		}

		protected SmartNotifyPropertyChangedAttribute(object instance, string propertyName)
		{
			Instance = instance;
			_propertyName = propertyName;

			var raisePropertyMethodInfo = GetRaiseMethod(Instance.GetType());
			_raisePropertyChanged = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), Instance, raisePropertyMethodInfo, true);
		}

		/// <inheritdoc/>
		public sealed override void OnGetValue(LocationInterceptionArgs args)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			if (this == null) //Look at NullAspectTest for more explanation.
			{
				args.ProceedGetValue();
				return;
			}

			ReactiveManager.Evaluate(this, () =>
			{
				args.ProceedGetValue();
				return true;
			});
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return "Sink from: " + Instance.GetType().Name + "." + _propertyName;
		}

		public void Notify()
		{
			_raisePropertyChanged(_propertyName);
		}

		/// <summary>
		/// Useful for debugging.
		/// </summary>
		// ReSharper disable once UnusedMember.Local
		private string PropertyName => _propertyName;

		/// <summary>
		/// Useful for debugging.
		/// </summary>
		// ReSharper disable once UnusedMember.Local
		private object Instance
		{
			get;
		}

		/// <inheritdoc/>
		public override object CreateInstance(AdviceArgs adviceArgs)
		{
			return new SmartNotifyPropertyChangedAttribute(adviceArgs.Instance, _propertyName);
		}

		/// <inheritdoc/>
		public override void CompileTimeInitialize(LocationInfo targetLocation, AspectInfo aspectInfo)
		{
			_propertyName = targetLocation.Name;
			base.CompileTimeInitialize(targetLocation, aspectInfo);
		}

		/// <inheritdoc/>
		public override void RuntimeInitializeInstance()
		{
		}
	}
}
