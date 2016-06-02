namespace SmartReactives.Core
{
    /// <summary>
    /// Used in a <see cref="WeakStrongReactive"/>
    /// </summary>
    public interface IWeakListener
    {
        void Notify(object strongKey);
    }
}