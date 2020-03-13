using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Camelot.Services.Operations.Implementations
{
    public class RemovingOperation : OperationBase
    {
        private readonly string _fileToRemove;

        public RemovingOperation(string fileToRemove)
        {
            if (string.IsNullOrWhiteSpace(fileToRemove))
            {
                throw new ArgumentNullException(nameof(fileToRemove));
            }

            _fileToRemove = fileToRemove;
        }

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: move to trash
            File.Delete(_fileToRemove);

            FireOperationFinishedEvent();

            return Task.CompletedTask;
        }
    }
}