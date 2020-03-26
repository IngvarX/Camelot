using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.TaskPool.Interfaces;

namespace Camelot.TaskPool.Implementations
{
    public class TaskPool : ITaskPool
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public TaskPool(int tasksCount)
        {
            _semaphoreSlim = new SemaphoreSlim(tasksCount);
        }

        public async Task ExecuteAsync(Func<Task> taskFactory)
        {
            await _semaphoreSlim.WaitAsync();

            Task
                .Run(taskFactory)
                .ContinueWith(task => _semaphoreSlim.Release())
                .Forget();
        }
    }
}