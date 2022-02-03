using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations;

public class CopyOperation : StatefulOperationWithProgressBase, IInternalOperation, ISelfBlockingOperation
{
    private readonly IDirectoryService _directoryService;
    private readonly IFileService _fileService;
    private readonly IPathService _pathService;
    private readonly string _sourceFile;
    private readonly string _destinationFile;

    private CancellationToken _cancellationToken;

    public (string SourceFilePath, string DestinationFilePath) CurrentBlockedFile { get; private set; }

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
        _cancellationToken = cancellationToken;
        _cancellationToken.ThrowIfCancellationRequested();

        CreateOutputDirectoryIfNeeded(_destinationFile);

        if (_fileService.CheckIfExists(_destinationFile))
        {
            if (_sourceFile == _destinationFile)
            {
                State = OperationState.Skipped;
            }
            else
            {
                CurrentBlockedFile = (_sourceFile, _destinationFile);
                State = OperationState.Blocked;
            }
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
            case OperationContinuationMode.OverwriteIfOlder:
                var sourceFileDateTime = GetLastModifiedDateTime(_sourceFile);
                var destinationFileDateTime = GetLastModifiedDateTime(_destinationFile);
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
                await CopyFileAsync(options.NewFilePath);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(options.Mode), options.Mode, null);
        }

        SetFinalProgress();
    }

    private void CreateOutputDirectoryIfNeeded(string destinationFile)
    {
        var outputDirectory = _pathService.GetParentDirectory(destinationFile);
        if (!_directoryService.CheckIfExists(outputDirectory))
        {
            _directoryService.Create(outputDirectory);
        }
    }

    private DateTime GetLastModifiedDateTime(string filePath) =>
        _fileService.GetFile(filePath).LastModifiedDateTime;

    private async Task CopyFileAsync(string destinationFile, bool force = false)
    {
        try
        {
            _cancellationToken.ThrowIfCancellationRequested();

            State = OperationState.InProgress;
            var isCopied = await _fileService.CopyAsync(_sourceFile, destinationFile, _cancellationToken, force);
            State = isCopied ? OperationState.Finished : OperationState.Failed;
            SetFinalProgress();
        }
        catch (TaskCanceledException)
        {
            State = OperationState.Cancelled;

            throw;
        }
    }
}