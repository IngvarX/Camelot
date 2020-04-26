using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Environment.Interfaces;
using Camelot.TaskPool.Interfaces;

namespace Camelot.TaskPool.Implementations
{
    public class TaskPool : ITaskPool
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public TaskPool(IEnvironmentService environmentService)
        {
            _semaphoreSlim = new SemaphoreSlim(environmentService.ProcessorsCount);
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