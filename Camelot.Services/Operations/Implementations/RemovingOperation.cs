using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Camelot.Services.Operations.Implementations
{
    public class RemovingOperation : OperationBase
    {
        private readonly string _pathToRemove;

        public RemovingOperation(string pathToRemove)
        {
            if (string.IsNullOrWhiteSpace(pathToRemove))
            {
                throw new ArgumentNullException(nameof(pathToRemove));
            }

            _pathToRemove = pathToRemove;
        }

        public override Task RunAsync(CancellationToken cancellationToken)
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

            FireOperationFinishedEvent();

            return Task.CompletedTask;
        }
    }
}