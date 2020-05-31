using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations
{
    public class CopyOperation : OperationWithProgress, IInternalOperation
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
            _directoryService = directoryService;
            _fileService = fileService;
            _pathService = pathService;
            _sourceFile = sourceFile;
            _destinationFile = destinationFile;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            CreateOutputDirectoryIfNeeded(_destinationFile);

            if (_fileService.CheckIfExists(_destinationFile))
            {
                State = OperationState.Blocked;
            }
            else
            {
                await CopyFileAsync();
            }
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

        private async Task CopyFileAsync()
        {
            try
            {
                State = OperationState.InProgress;
                await _fileService.CopyAsync(_sourceFile, _destinationFile);
                State = OperationState.Finished;
            }
            catch
            {
                // TODO: process
                State = OperationState.Failed;
            }
        }
    }
}