using SmartReactives.Core;

namespace SmartReactives.Test
{
    class NotifyCounter : IListener
    {
        public int Counter { get; set; }

        public void Notify()
        {
            Counter++;
        }
    }
}