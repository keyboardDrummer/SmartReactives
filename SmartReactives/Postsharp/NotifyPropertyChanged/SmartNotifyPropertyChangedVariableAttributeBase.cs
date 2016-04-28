using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PostSharp;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Reflection;
using SmartReactives.Core;

namespace SmartReactives.Postsharp.NotifyPropertyChanged
{
	/// <summary>
	/// Base class for reactive aspects for a property that is a dependency of other properties.
	/// </summary>
	[Serializable]
	public abstract class SmartNotifyPropertyChangedVariableAttributeBase : LocationInterceptionAspect, IInstanceScopedAspect
	{
		PropertyInfo property;

		/// <summary>
		/// Useful for debugging.
		/// </summary>
		// ReSharper disable once UnusedMember.Local
		IEnumerable<object> Dependents => ReactiveManager.GetDependents(this);

		/// <inheritdoc/>
		public override string ToString()
		{
			// ReSharper disable once PossibleNullReferenceException
			return "Source from: " + property.DeclaringType.Name + "." + property.Name;
		}

		public static MethodInfo GetRaiseMethod(Type type)
		{
			return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance).
				FirstOrDefault(method => method.GetCustomAttributes(typeof(NotifyPropertyChangedInvocator2Attribute), true).Any());
		}

		/// <inheritdoc/>
		public sealed override void OnSetValue(LocationInterceptionArgs args)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			if (this == null) //This is because of the case in TestPropertyAccessDuringConstructionReflection
			{
				args.ProceedSetValue();
				return;
			}

			var newValue = args.Value;
			var oldValue = args.GetCurrentValue();

			if (Equals(newValue, oldValue))
			{
				return;
			}
			args.SetNewValue(newValue);
			ReactiveManager.WasChanged(this);
		}

		public override bool Equals(object obj)
		{
			return RuntimeHelpers.Equals(this, obj);
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public override bool CompileTimeValidate(LocationInfo locationInfo)
		{
			ValidateHasRaiseMethod(locationInfo);
			return base.CompileTimeValidate(locationInfo);
		}

		public static void ValidateHasRaiseMethod(LocationInfo locationInfo)
		{
			var method = GetRaiseMethod(locationInfo.DeclaringType);
			if (method == null || method.ReturnType != typeof(void) || !method.GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual(new[] {typeof(string)}))
			{
				var message = $"{locationInfo.DeclaringType.Name} should contains a method annotated with " +
				              $"{typeof(NotifyPropertyChangedInvocator2Attribute).Name} that has a signature like {typeof(Action<string>).Name} " +
				              $"if it uses the {typeof(SmartNotifyPropertyChangedAttribute).Name} attribute on a property.";
				MessageSource.MessageSink.Write(new Message(MessageLocation.Of(locationInfo.PropertyInfo),
					SeverityType.Error, "AF0001", message, "", locationInfo.DeclaringType.Assembly.ToString(), null));
			}
		}

		public override void CompileTimeInitialize(LocationInfo targetLocation, AspectInfo aspectInfo)
		{
			property = targetLocation.PropertyInfo;
			base.CompileTimeInitialize(targetLocation, aspectInfo);
		}

		public abstract object CreateInstance(AdviceArgs adviceArgs);
		public abstract void RuntimeInitializeInstance();
	}
}
