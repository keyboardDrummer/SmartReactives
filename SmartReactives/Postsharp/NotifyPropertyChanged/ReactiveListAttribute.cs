using System;
using System.Collections.Specialized;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using SmartReactives.List;

namespace SmartReactives.Postsharp.NotifyPropertyChanged
{
	/// <summary>
	/// Makes a list property reactive.
	/// </summary>
	[Serializable]
	public class ReactiveListAttribute : LocationInterceptionAspect
	{
		/// <inheritdoc/>
		public sealed override void OnGetValue(LocationInterceptionArgs args)
		{
			ReactiveManagerWithList.Evaluate(() =>
			{
				base.OnGetValue(args);
				return (INotifyCollectionChanged)args.GetCurrentValue();
			});
		}
	}
}
