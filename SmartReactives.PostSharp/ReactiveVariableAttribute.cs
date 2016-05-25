using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;
using SmartReactives.Core;

namespace SmartReactives.PostSharp
{
	/// <summary>
	/// Allows this property to be used by the reactive framework.
	/// </summary>
	[Serializable]
	[AttributeUsage(AttributeTargets.Property)]
	public class ReactiveVariableAttribute : LocationInterceptionAspect, IInstanceScopedAspect
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
		public sealed override void OnSetValue(LocationInterceptionArgs args)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			if (this == null) //This is because of the case in TestPropertyAccessDuringConstructionReflection
			{
				args.ProceedSetValue();
				return;
			}

			args.ProceedSetValue();
			ReactiveManager.WasChanged(this);
		}


		public object CreateInstance(AdviceArgs adviceArgs)
		{
			return new ReactiveVariableAttribute();
		}

		public void RuntimeInitializeInstance()
		{
		}
	}
}
