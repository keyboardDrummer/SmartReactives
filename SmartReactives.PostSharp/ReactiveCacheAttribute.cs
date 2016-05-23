using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;
using SmartReactives.Extensions;

namespace SmartReactives.Postsharp
{
	class ReactiveCacheAttribute : LocationInterceptionAspect, IInstanceScopedAspect
	{
		ReactiveCache<object> cache;

		public override void OnGetValue(LocationInterceptionArgs args)
		{
			cache = cache ?? new ReactiveCache<object>(args.GetCurrentValue);
			args.Value = cache.Get();
		}

		public object CreateInstance(AdviceArgs adviceArgs)
		{
			return new ReactiveCacheAttribute();
		}

		public void RuntimeInitializeInstance()
		{
		}
	}
}
