using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;

namespace SmartReactives.Postsharp
{
	class ReactiveCacheAttribute : LocationInterceptionAspect, IInstanceScopedAspect
	{
		public override void OnGetValue(LocationInterceptionArgs args)
		{
			base.OnGetValue(args);
		}
	}
}
