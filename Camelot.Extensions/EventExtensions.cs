using System;
using System.Threading;

namespace Camelot.Extensions
{
    public static class EventExtensions
    {
        public static void Raise<TArgs>(
            this EventHandler<TArgs> eventHandler,
            object sender,
            TArgs args) where TArgs : EventArgs
        {
            var handler = Volatile.Read(ref eventHandler);

            handler?.Invoke(sender, args);
        }
    }
}