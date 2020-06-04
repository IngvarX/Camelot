using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations
{
    public class CopyOperation : OperationBase, IInternalOperation, ISelfBlockingOperation
    {
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly string _sourceFile;
        private readonly string _destinationFile;

        private readonly IList<(string SourceFilePath, string DestinationFilePath)> _blockedFiles;

        public IReadOnlyList<(string SourceFilePath, string DestinationFilePath)> BlockedFiles => _blockedFiles.ToArray();

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

            _blockedFiles = new List<(string SourceFilePath, string DestinationFilePath)>();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            CreateOutputDirectoryIfNeeded(_destinationFile);

            if (_fileService.CheckIfExists(_destinationFile))
            {
                _blockedFiles.Add((_sourceFile, _destinationFile));
                State = OperationState.Blocked;
            }
            else
            {
                await CopyFileAsync(_destinationFile);
            }
        }

        public async Task ContinueAsync(OperationContinuationOptions options)
        {
            switch (options.Mode)
            {
                case OperationContinuationMode.Skip:
                    State = OperationState.Skipped;
                    break;
                case OperationContinuationMode.Overwrite:
                    await CopyFileAsync(_destinationFile, true);
                    break;
                case OperationContinuationMode.OverwriteOlder:
                    var sourceFileDateTime = _fileService.GetFile(_sourceFile).LastWriteTime;;
                    var destinationFileDateTime = _fileService.GetFile(_destinationFile).LastWriteTime;
                    if (sourceFileDateTime > destinationFileDateTime)
                    {
                        await CopyFileAsync(_destinationFile, true);
                    }
                    else
                    {
                        State = OperationState.Skipped;
                    }
                    break;
                case OperationContinuationMode.Rename:
                    await CopyFileAsync(options.NewFileName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options.Mode));
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

        private async Task CopyFileAsync(string destinationFile, bool force = false)
        {
            try
            {
                State = OperationState.InProgress;
                await _fileService.CopyAsync(_sourceFile, destinationFile, force);
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