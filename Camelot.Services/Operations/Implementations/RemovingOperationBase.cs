using System;
using System.IO;
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
            // TODO: move to trash
            if (File.Exists(_pathToRemove))
            {
                File.Delete(_pathToRemove);
            }
            else
            {
                // TODO: preprocess in operations factory?
                Directory.Delete(_pathToRemove, true);
            }

            await RemoveAsync(_pathToRemove);

            FireOperationFinishedEvent();
        }

        protected abstract Task RemoveAsync(string pathToRemove);
    }
}