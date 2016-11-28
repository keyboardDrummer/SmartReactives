namespace SmartReactives.Core
{
    /// <summary>
    /// A reactive object that consists of both a weak and a strong part.
    /// The weak part is only weakly referenced by <see cref="ReactiveManager"/> while the strong part is strongly referenced.
    /// If the weak part is no longer referenced from outside <see cref="ReactiveManager"/>, then <see cref="ReactiveManager"/> forgets the entire <see cref="CompositeReactiveObject"/>
    /// </summary>
    public struct CompositeReactiveObject : IListener
    {
        public CompositeReactiveObject(object weak, object strong)
        {
            Weak = weak;
            Strong = strong;
        }

        public object Weak { get; }
        public object Strong { get; }

        public void Notify()
        {
            (Weak as IWeakListener)?.Notify(Strong);
        }

	    public bool StrongReference => false;
    }
}