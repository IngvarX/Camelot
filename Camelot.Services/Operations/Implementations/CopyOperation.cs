using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Operations.Implementations
{
    public class CopyOperation : OperationBase
    {
        private readonly IDirectoryService _directoryService;
        private readonly string _sourceFile;
        private readonly string _destinationFile;

        public CopyOperation(
            IDirectoryService directoryService,
            string sourceFile,
            string destinationFile)
        {
            if (string.IsNullOrWhiteSpace(sourceFile))
            {
                throw new ArgumentNullException(nameof(sourceFile));
            }

            if (string.IsNullOrWhiteSpace(destinationFile))
            {
                throw new ArgumentNullException(nameof(destinationFile));
            }

            _directoryService = directoryService;
            _sourceFile = sourceFile;
            _destinationFile = destinationFile;
        }

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            CreateOutputDirectoryIfNeeded(_destinationFile);

            File.Copy(_sourceFile, _destinationFile);

            FireOperationFinishedEvent();

            return Task.CompletedTask;
        }

        private void CreateOutputDirectoryIfNeeded(string destinationFile)
        {
            try
            {
                var outputDirectory = Path.GetDirectoryName(destinationFile);
                if (!_directoryService.CheckIfDirectoryExists(outputDirectory))
                {
                    _directoryService.CreateDirectory(outputDirectory);
                }
            }
            catch
            {
                // ignore
            }
        }
    }
}