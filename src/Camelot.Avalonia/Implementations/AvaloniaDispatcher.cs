using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Camelot.Avalonia.Interfaces;

namespace Camelot.Avalonia.Implementations
{
    public class AvaloniaDispatcher : IApplicationDispatcher
    {
        private static Dispatcher Dispatcher => Dispatcher.UIThread;

        public void Dispatch(Action action) => Dispatcher.Post(action);

        public Task DispatchAsync(Func<Task> task) => Dispatcher.InvokeAsync(task);
    }
}