namespace SmartReactives.Core
{
    /// <summary>
    /// Used in a <see cref="CompositeReactiveObject"/>
    /// </summary>
    public interface IWeakListener
    {
        void Notify(object strongKey);
    }
}