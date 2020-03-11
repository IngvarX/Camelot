using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Camelot.Services.Operations.Implementations
{
    public class CopyOperation : OperationBase
    {
        private readonly IList<(string Source, string Destination)> _filesToCopy;

        public CopyOperation(IList<(string Source, string Destination)> filesToCopy)
        {
            _filesToCopy = filesToCopy;
        }

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: parallel
            var filesCount = _filesToCopy.Count;
            for (var i = 0; i < filesCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var (source, destination) = _filesToCopy[i];
                File.Copy(source, destination);

                var currentProgress = (double)i / filesCount;
                FireProgressChangedEvent(currentProgress);
            }

            FireOperationFinishedEvent();

            return Task.CompletedTask;
        }
    }
}