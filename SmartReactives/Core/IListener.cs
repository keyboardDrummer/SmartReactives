namespace SmartReactives.Core
{
    /// <summary>
    /// Listens to notifications.
    /// </summary>
    public interface IListener
    {
        /// <summary>
        /// Notifies the listener.
        /// </summary>
        void Notify();
    }
}