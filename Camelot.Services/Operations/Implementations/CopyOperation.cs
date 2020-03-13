using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Camelot.Services.Operations.Implementations
{
    public class CopyOperation : OperationBase
    {
        private readonly string _sourceFile;
        private readonly string _destinationFile;

        public CopyOperation(string sourceFile, string destinationFile)
        {
            if (string.IsNullOrWhiteSpace(sourceFile))
            {
                throw new ArgumentNullException(nameof(sourceFile));
            }

            if (string.IsNullOrWhiteSpace(destinationFile))
            {
                throw new ArgumentNullException(nameof(destinationFile));
            }

            _sourceFile = sourceFile;
            _destinationFile = destinationFile;
        }

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            File.Copy(_sourceFile, _destinationFile);

            FireOperationFinishedEvent();

            return Task.CompletedTask;
        }
    }
}