using System;
using System.Collections.Generic;
using PostSharp.Aspects;
using PostSharp.Reflection;
using SmartReactives.Core;

namespace SmartReactives.Postsharp.NotifyPropertyChanged
{
	/// <summary>
	/// An expression property is one that has only a getter containing an expression.
	/// Place this attribute on a dependent property to make it reactive.
	/// </summary>
	[Serializable]
	public class SmartNotifyPropertyChangedExpressionAttribute : LocationInterceptionAspect, IInstanceScopedAspect, IListener
	{
		protected string _propertyName;
		private readonly Action<string> _raisePropertyChanged;

		public SmartNotifyPropertyChangedExpressionAttribute()
		{
		}

		protected SmartNotifyPropertyChangedExpressionAttribute(object instance, string propertyName)
		{
			Instance = instance;
			_propertyName = propertyName;

			var raisePropertyMethodInfo = SmartNotifyPropertyChangedVariableAttributeBase.GetRaiseMethod(Instance.GetType());
			_raisePropertyChanged = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), Instance, raisePropertyMethodInfo, true);
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

		/// <summary>
		/// Useful for debugging.
		/// </summary>
		// ReSharper disable once UnusedMember.Local
		private IEnumerable<object> Dependents => ReactiveManager.GetDependents(this);

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

		/// <inheritdoc/>
		public void Notify()
		{
			_raisePropertyChanged(_propertyName);
		}

		/// <inheritdoc/>
		public virtual object CreateInstance(AdviceArgs adviceArgs)
		{
			return new SmartNotifyPropertyChangedExpressionAttribute(adviceArgs.Instance, _propertyName);
		}

		/// <inheritdoc/>
		public override void CompileTimeInitialize(LocationInfo targetLocation, AspectInfo aspectInfo)
		{
			_propertyName = targetLocation.Name;
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
