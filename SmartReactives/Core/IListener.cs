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

		/// <summary>
		/// Whether to keep a strong reference to the listener as long as it depends on something.
		/// </summary>
		bool StrongReference { get; }
    }
}