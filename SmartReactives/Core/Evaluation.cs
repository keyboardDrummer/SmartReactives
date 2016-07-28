namespace SmartReactives.Core
{
    /// <summary>
    /// State for evaluation of a reactive expression. Contains the reactive <see cref="IListener"/> which is evaluating, and a cached <see cref="Dependency"/> targetting that reactive.
    /// </summary>
    class Evaluation
    {
        readonly IListener dependent;
		IDependency dependentReference;

        public Evaluation(IListener dependent)
        {
            this.dependent = dependent;
        }

        public IDependency Dependency
        {
            get
            {
                if (dependentReference == null)
                {
                    var node = ReactiveManager.GetNode(dependent);
                    dependentReference = node.GetDependency(dependent);
                }
                return dependentReference;
            }
        }
    }
}