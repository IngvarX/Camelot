using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Operations.Implementations
{
    public class CopyOperation : OperationBase
    {
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly string _sourceFile;
        private readonly string _destinationFile;

        public CopyOperation(
            IDirectoryService directoryService,
            IFileService fileService,
            IPathService pathService,
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
            _fileService = fileService;
            _pathService = pathService;
            _sourceFile = sourceFile;
            _destinationFile = destinationFile;
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            CreateOutputDirectoryIfNeeded(_destinationFile);

            await _fileService.CopyAsync(_sourceFile, _destinationFile);

            FireOperationFinishedEvent();
        }

        private void CreateOutputDirectoryIfNeeded(string destinationFile)
        {
            try
            {
                var outputDirectory = _pathService.GetParentDirectory(destinationFile);
                if (!_directoryService.CheckIfExists(outputDirectory))
                {
                    _directoryService.Create(outputDirectory);
                }
            }
            catch
            {
                // ignore
            }
        }
    }
}