using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Exceptions;
using Camelot.Services.Abstractions.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Operations.Models;

namespace Camelot.Services.Operations;

public class CompositeOperation : OperationWithProgressBase, ICompositeOperation
{
    private readonly IFileNameGenerationService _fileNameGenerationService;
    private readonly IReadOnlyList<OperationGroup> _groupedOperationsToExecute;

    private readonly IDictionary<string, ISelfBlockingOperation> _blockedOperationsDictionary;
    private readonly ConcurrentQueue<(string SourceFilePath, string DestinationFilePath)> _blockedFilesQueue;
    private readonly object _locker;

    private int _finishedOperationsCount;
    private int _currentOperationsGroupIndex;
    private int _operationsGroupsCount;
    private IReadOnlyList<IInternalOperation> _currentOperationsGroup;
    private int _totalOperationsCount;
    private CancellationTokenSource _cancellationTokenSource;
    private TaskCompletionSource<bool> _taskCompletionSource;
    private OperationContinuationMode? _continuationMode;

    public OperationInfo Info { get; }

    public (string SourceFilePath, string DestinationFilePath) CurrentBlockedFile =>
        _blockedFilesQueue.TryPeek(out var blockedFile) ? blockedFile : default;

    public event EventHandler<EventArgs> Blocked;

    public CompositeOperation(
        IFileNameGenerationService fileNameGenerationService,
        IReadOnlyList<OperationGroup> groupedOperationsToExecute,
        OperationInfo operationInfo)
    {
        _fileNameGenerationService = fileNameGenerationService;
        _groupedOperationsToExecute = groupedOperationsToExecute;

        Info = operationInfo;

        _blockedOperationsDictionary = new ConcurrentDictionary<string, ISelfBlockingOperation>();
        _blockedFilesQueue = new ConcurrentQueue<(string SourceFilePath, string DestinationFilePath)>();
        _locker = new object();
    }

    public async Task RunAsync()
    {
        var operations = _groupedOperationsToExecute.Select(g => g.Operations).ToArray();

        await ExecuteOperationsAsync(operations);
    }

    public async Task ContinueAsync(OperationContinuationOptions options)
    {
        if (options.ApplyToAll)
        {
            _continuationMode = options.Mode;
        }

        _blockedFilesQueue.TryDequeue(out _);

        var operation = _blockedOperationsDictionary[options.FilePath];
        _blockedOperationsDictionary.Remove(options.FilePath);

        await operation.ContinueAsync(options);
        await ProcessNextBlockedTaskAsync();
    }

    public Task PauseAsync()
    {
        throw new NotImplementedException();
    }

    public Task UnpauseAsync()
    {
        throw new NotImplementedException();
    }

    public async Task CancelAsync()
    {
        _cancellationTokenSource.Cancel();

        while (_blockedFilesQueue.Any())
        {
            await ProcessNextBlockedTaskAsync();
        }

        await _taskCompletionSource.Task;
    }

    private async Task ExecuteOperationsAsync(
        IReadOnlyList<IReadOnlyList<IInternalOperation>> groupedOperationsToExecute)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        _totalOperationsCount = groupedOperationsToExecute.Sum(g => g.Count);
        _operationsGroupsCount = groupedOperationsToExecute.Count;
        _currentOperationsGroupIndex = 0;

        foreach (var operationsGroup in groupedOperationsToExecute)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!operationsGroup.Any())
            {
                continue;
            }

            _taskCompletionSource = new TaskCompletionSource<bool>();

            _finishedOperationsCount = 0;
            _currentOperationsGroup = operationsGroup;

            for (var i = 0; i < operationsGroup.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var currentOperation = operationsGroup[i];

                var previousOperationFailed =
                    _currentOperationsGroupIndex > 0
                    && groupedOperationsToExecute[_currentOperationsGroupIndex - 1][i].State != OperationState.Finished;
                if (previousOperationFailed)
                {
                    FinishOperation(currentOperation);

                    continue;
                }

                SubscribeToEvents(currentOperation);

                RunOperation(currentOperation, cancellationToken);
            }

            var groupExecutionResult = await _taskCompletionSource.Task;
            if (!groupExecutionResult)
            {
                throw new OperationFailedException();
            }

            _currentOperationsGroupIndex++;
        }

        _currentOperationsGroup = null;
        SetFinalProgress();
    }

    private void RunOperation(IInternalOperation operation, CancellationToken cancellationToken) =>
        Task
            .Run(() => operation.RunAsync(cancellationToken), cancellationToken)
            .ContinueWith(t =>
            {
                if (t.IsCanceled && operation.State == OperationState.NotStarted)
                {
                    FinishOperation(operation);
                }
            });

    private async void OperationOnStateChanged(object sender, OperationStateChangedEventArgs e)
    {
        var state = e.OperationState;

        if (state is OperationState.Blocked)
        {
            var operation = (ISelfBlockingOperation) sender;
            await ProcessBlockedTaskAsync(operation);

            return;
        }

        if (state.IsCompleted())
        {
            var operation = (IInternalOperation) sender;

            FinishOperation(operation);
        }

        UpdateProgress();
    }

    private void FinishOperation(IInternalOperation operation)
    {
        UnsubscribeFromEvents(operation);

        var finishedOperationsCount = Interlocked.Increment(ref _finishedOperationsCount);
        if (finishedOperationsCount != _currentOperationsGroup?.Count)
        {
            return;
        }

        var isSuccessful = _currentOperationsGroup.All(o => !operation.State.IsFailedOrCancelled());

        _taskCompletionSource.SetResult(isSuccessful);
    }

    private async Task ProcessNextBlockedTaskAsync()
    {
        string sourceFilePath;
        lock (_locker)
        {
            var isBlockedFileAvailable = _blockedFilesQueue.TryDequeue(out var currentBlockedFile);
            if (!isBlockedFileAvailable)
            {
                return;
            }

            sourceFilePath = currentBlockedFile.SourceFilePath;
        }

        var operation = _blockedOperationsDictionary[sourceFilePath];
        _blockedOperationsDictionary.Remove(sourceFilePath);

        await ProcessBlockedTaskAsync(operation);
    }

    private async Task ProcessBlockedTaskAsync(ISelfBlockingOperation operation)
    {
        if (_cancellationTokenSource.IsCancellationRequested)
        {
            FinishOperation((IInternalOperation) operation);

            return;
        }

        if (_continuationMode is null)
        {
            _blockedOperationsDictionary[operation.CurrentBlockedFile.SourceFilePath] = operation;
            _blockedFilesQueue.Enqueue(operation.CurrentBlockedFile);

            lock (_locker)
            {
                if (CurrentBlockedFile == operation.CurrentBlockedFile && _continuationMode is null)
                {
                    Blocked.Raise(this, EventArgs.Empty);
                }
            }
        }
        else
        {
            await ContinueWithDefaultOptionsAsync(operation, _continuationMode.Value);
        }
    }

    private async Task ContinueWithDefaultOptionsAsync(ISelfBlockingOperation operation,
        OperationContinuationMode continuationMode)
    {
        var (sourceFilePath, destinationFilePath) = operation.CurrentBlockedFile;
        OperationContinuationOptions options;
        if (continuationMode is OperationContinuationMode.Rename)
        {
            var newFilePath = _fileNameGenerationService.GenerateFullName(destinationFilePath);
            options = OperationContinuationOptions.CreateRenamingContinuationOptions(
                sourceFilePath,
                true,
                newFilePath
            );
        }
        else
        {
            options = OperationContinuationOptions.CreateContinuationOptions(
                sourceFilePath,
                true,
                continuationMode
            );
        }

        await operation.ContinueAsync(options);
    }

    private void SubscribeToEvents(IInternalOperation currentOperation)
    {
        currentOperation.StateChanged += OperationOnStateChanged;
        currentOperation.ProgressChanged += OperationOnProgressChanged;
    }

    private void UnsubscribeFromEvents(IInternalOperation currentOperation)
    {
        currentOperation.StateChanged -= OperationOnStateChanged;
        currentOperation.ProgressChanged += OperationOnProgressChanged;
    }

    private void OperationOnProgressChanged(object sender, OperationProgressChangedEventArgs e) =>
        UpdateProgress();

    private void UpdateProgress()
    {
        if (_currentOperationsGroup is null)
        {
            return;
        }

        var finishedOperationGroupsProgress = (double) _currentOperationsGroupIndex;
        var currentOperationGroupProgress = _currentOperationsGroup.Sum(o => o.CurrentProgress) / _totalOperationsCount;

        CurrentProgress = (finishedOperationGroupsProgress + currentOperationGroupProgress) / _operationsGroupsCount;
    }
}