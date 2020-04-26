using System;
using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;
using Avalonia.Threading;

namespace ApplicationDispatcher.Implementations
{
    public class AvaloniaDispatcher : IApplicationDispatcher
    {
        private static Dispatcher Dispatcher => Dispatcher.UIThread;

        public void Dispatch(Action action) => Dispatcher.Post(action);
        
        public void Dispatch(Func<Task> task) => Dispatcher.InvokeAsync(task);
    }
}