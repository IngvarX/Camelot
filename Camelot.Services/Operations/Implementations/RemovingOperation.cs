using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Camelot.Services.Operations.Implementations
{
    public class RemovingOperation : OperationBase
    {
        private readonly IList<string> _filesToRemove;

        public RemovingOperation(IList<string> filesToRemove)
        {
            _filesToRemove = filesToRemove;
        }

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            var filesCount = _filesToRemove.Count;
            for (var i = 0; i < filesCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                File.Delete(_filesToRemove[i]);

                var currentProgress = (double)i / filesCount;
                FireProgressChangedEvent(currentProgress);
            }

            FireOperationFinishedEvent();

            return Task.CompletedTask;
        }
    }
}