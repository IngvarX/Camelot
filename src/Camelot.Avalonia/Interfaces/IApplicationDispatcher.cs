using System;
using System.Threading.Tasks;

namespace Camelot.Avalonia.Interfaces
{
    public interface IApplicationDispatcher
    {
        void Dispatch(Action action);

        Task DispatchAsync(Func<Task> task);
    }
}