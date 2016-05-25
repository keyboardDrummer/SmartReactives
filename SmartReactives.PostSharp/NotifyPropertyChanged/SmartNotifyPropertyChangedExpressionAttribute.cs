using System;
using System.Collections.Generic;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using PostSharp.Reflection;
using SmartReactives.Core;

namespace SmartReactives.PostSharp.NotifyPropertyChanged
{
	/// <summary>
	/// An expression property is one that has only a getter containing an expression.
	/// Place this attribute on a dependent property to make it reactive.
	/// </summary>
	[Serializable]
	[ProvideAspectRole("SmartNotifyPropertyChanged")]
	public class SmartNotifyPropertyChangedExpressionAttribute : LocationInterceptionAspect, IInstanceScopedAspect, IListener
	{
		readonly Action<string> raisePropertyChanged;

		public SmartNotifyPropertyChangedExpressionAttribute()
		{
		}

		protected SmartNotifyPropertyChangedExpressionAttribute(object instance, string propertyName)
		{
			Instance = instance;
			PropertyName = propertyName;

			var raisePropertyMethodInfo = SmartNotifyPropertyChangedVariableAttributeBase.GetRaiseMethod(Instance.GetType());
			raisePropertyChanged = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), Instance, raisePropertyMethodInfo, true);
		}

		/// <summary>
		/// Useful for debugging.
		/// </summary>
		// ReSharper disable once UnusedMember.Local
		public string PropertyName { get; private set; }

		/// <summary>
		/// Useful for debugging.
		/// </summary>
		// ReSharper disable once UnusedMember.Local
		public object Instance
		{
			get;
		}

		/// <summary>
		/// Useful for debugging.
		/// </summary>
		// ReSharper disable once UnusedMember.Local
		public IEnumerable<object> Dependents => ReactiveManager.GetDependents(this);

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
			return "Sink from: " + Instance.GetType().Name + "." + PropertyName;
		}

		/// <inheritdoc/>
		public void Notify()
		{
			raisePropertyChanged(PropertyName);
		}

		/// <inheritdoc/>
		public virtual object CreateInstance(AdviceArgs adviceArgs)
		{
			return new SmartNotifyPropertyChangedExpressionAttribute(adviceArgs.Instance, PropertyName);
		}

		/// <inheritdoc/>
		public override void CompileTimeInitialize(LocationInfo targetLocation, AspectInfo aspectInfo)
		{
			PropertyName = targetLocation.Name;
			base.CompileTimeInitialize(targetLocation, aspectInfo);
		}

		public override bool CompileTimeValidate(LocationInfo locationInfo)
		{
			SmartNotifyPropertyChangedVariableAttributeBase.ValidateHasRaiseMethod(locationInfo);
			return base.CompileTimeValidate(locationInfo);
		}

		/// <inheritdoc/>
		public void RuntimeInitializeInstance()
		{
		}
	}
}
