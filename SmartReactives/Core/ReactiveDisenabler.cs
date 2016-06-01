using System;

namespace SmartReactives.Core
{
    /// <summary>
    /// Disables the reactive system for this particular thread.
    /// </summary>
    public class ReactiveDisenabler : IDisposable //TODO is this still useful?
    {
        private readonly bool _previous;

        /// <inheritdoc/>
        public ReactiveDisenabler(bool enable = false)
        {
            _previous = ReactiveManager.State.Enabled;
            ReactiveManager.State.Enabled = enable;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ReactiveManager.State.Enabled = _previous;
        }
    }
}