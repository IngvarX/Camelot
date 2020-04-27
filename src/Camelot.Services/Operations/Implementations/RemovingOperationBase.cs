using System;
using System.Threading;
using System.Threading.Tasks;

namespace Camelot.Services.Operations.Implementations
{
    public abstract class RemovingOperationBase : OperationBase
    {
        private readonly string _pathToRemove;

        protected RemovingOperationBase(string pathToRemove)
        {
            if (string.IsNullOrWhiteSpace(pathToRemove))
            {
                throw new ArgumentNullException(nameof(pathToRemove));
            }

            _pathToRemove = pathToRemove;
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            await RemoveAsync(_pathToRemove);

            FireOperationFinishedEvent();
        }

        protected abstract Task RemoveAsync(string pathToRemove);
    }
}