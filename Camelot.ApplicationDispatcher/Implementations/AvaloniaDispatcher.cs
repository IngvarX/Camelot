using System;
using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;

namespace ApplicationDispatcher.Implementations
{
    public class AvaloniaDispatcher : IApplicationDispatcher
    {
        public void Dispatch(Action action)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(action);
        }

        public void Dispatch(Func<Task> task)
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(task);
        }
    }
}