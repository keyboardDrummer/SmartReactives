using System;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using SmartReactives.Core;

namespace SmartReactives.Postsharp.NotifyPropertyChanged
{
	/// <summary>
	/// A variable property is one which uses a backing field, and does not dependend on other properties.
	/// Place this attribute on a variable property to enable dependencies on this property to be tracking automatically.
	/// </summary>
	[Serializable]
	[ProvideAspectRole("SmartNotifyPropertyChanged")]
	public class SmartNotifyPropertyChangedVariableAttribute : SmartNotifyPropertyChangedVariableAttributeBase
	{
		/// <inheritdoc/>
		public sealed override void OnGetValue(LocationInterceptionArgs args)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			if (this != null) //This is because of the case in TestPropertyAccessDuringConstructionReflection
			{
				ReactiveManager.WasRead(this);
			}

			args.ProceedGetValue();
		}

		/// <inheritdoc/>
		public override object CreateInstance(AdviceArgs adviceArgs)
		{
			return new SmartNotifyPropertyChangedVariableAttribute();
		}

		/// <inheritdoc/>
		public override void RuntimeInitializeInstance()
		{
		}
	}
}
