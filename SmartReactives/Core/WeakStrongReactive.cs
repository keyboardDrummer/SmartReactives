namespace SmartReactives.Core
{
    /// <summary>
    /// A reactive object that consists of both a weak and a strong part.
    /// The weak part is only weakly referenced by <see cref="ReactiveManager"/> while the strong part is strongly referenced.
    /// If the weak part is no longer referenced from outside <see cref="ReactiveManager"/>, then <see cref="ReactiveManager"/> forgets the entire <see cref="WeakStrongReactive"/>
    /// </summary>
    public struct WeakStrongReactive : IListener
    {
        public WeakStrongReactive(IWeakListener weak, object strong)
        {
            Weak = weak;
            Strong = strong;
        }

        public IWeakListener Weak { get; }
        public object Strong { get; }

        public void Notify()
        {
            Weak.Notify(Strong);
        }
    }
}