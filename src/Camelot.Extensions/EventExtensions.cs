using System;
using System.Threading;

namespace Camelot.Extensions
{
    public static class EventExtensions
    {
        public static void Raise<TEventArgs>(
            this EventHandler<TEventArgs> eventHandler,
            object sender,
            TEventArgs args) where TEventArgs : EventArgs
        {
            if (sender is null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var handler = Volatile.Read(ref eventHandler);

            handler?.Invoke(sender, args);
        }
    }
}