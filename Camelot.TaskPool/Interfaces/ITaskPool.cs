using System;
using System.Threading.Tasks;

namespace Camelot.TaskPool.Interfaces
{
    public interface ITaskPool
    {
        Task ExecuteAsync(Func<Task> taskFactory);
    }
}