using System;

namespace SmartReactives.Core
{
    /// <summary>
    /// Disables the reactive system for this particular thread.
    /// </summary>
    public class ReactiveDisenabler : IDisposable //TODO is this still useful?
    {
        readonly bool previous;

        /// <inheritdoc/>
        public ReactiveDisenabler(bool enable = false)
        {
            previous = ReactiveManager.State.Enabled;
            ReactiveManager.State.Enabled = enable;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ReactiveManager.State.Enabled = previous;
        }
    }
}