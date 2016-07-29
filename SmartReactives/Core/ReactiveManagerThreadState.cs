using System;
using System.Collections.Generic;

namespace SmartReactives.Core
{
    /// <summary>
    /// The thread specific state of <see cref="ReactiveManager" />
    /// </summary>
    class ReactiveManagerThreadState
    {
        Stack<Evaluation> DependentsEvaluating { get; } = new Stack<Evaluation>();

        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Must be called whenever a node value is changed externally, for example when a property node's backing field changes its value.
        /// </summary>
        public void WasChanged(object source)
        {
            if (!Enabled)
            {
                return;
            }

	        Chain<IListener> notifyChain = null;
            ReactiveManager.TryGetNode(source)?.NotifyChildren(ref notifyChain);
	        while (notifyChain != null)
	        {
		        notifyChain.Value.Notify();
		        notifyChain = notifyChain.Next;
	        }
            (source as IListener)?.Notify();
        }

        /// <summary>
        /// Indicate that the given variable was read. May cause other objects to become dependend on the given variable.
        /// </summary>
        public void WasRead(object source)
        {
            if (!Enabled)
            {
                return;
            }

            if (DependentsEvaluating.Count != 0)
            {
                var weakDependent = DependentsEvaluating.Peek().Dependency;
                ReactiveManager.GetNode(source).Add(weakDependent);
            }
        }

        /// <summary>
        /// Evaluation of reactive nodes must pass through this method.
        /// </summary>
        public T Evaluate<T>(IListener dependent, Func<T> func)
        {
            if (!Enabled)
            {
                return func();
            }

            WasRead(dependent);

            DependentsEvaluating.Push(new Evaluation(dependent));
	        try
	        {
		        return func();
			}
	        finally
			{
				DependentsEvaluating.Pop();
			}
        }
    }
}