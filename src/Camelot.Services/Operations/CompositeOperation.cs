using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.TaskPool.Interfaces;

namespace Camelot.Services.Operations
{
    public class CompositeOperation : OperationBase, IOperation
    {
        private readonly ITaskPool _taskPool;
        private readonly IReadOnlyList<OperationGroup> _groupedOperationsToExecute;

        private int _finishedOperationsCount;
        private IReadOnlyList<IInternalOperation> _currentOperationsGroup;
        private int _totalOperationsCount;
        private CancellationTokenSource _cancellationTokenSource;
        private TaskCompletionSource<bool> _taskCompletionSource;

        public OperationInfo Info { get; }

        public CompositeOperation(
            ITaskPool taskPool,
            IReadOnlyList<OperationGroup> groupedOperationsToExecute,
            OperationInfo operationInfo)
        {
            _taskPool = taskPool;
            _groupedOperationsToExecute = groupedOperationsToExecute;
            Info = operationInfo;
        }

        public Task RunAsync() =>
            ChangeStateAsync(OperationState.NotStarted, OperationState.InProgress);

        public Task ContinueAsync(OperationContinuationOptions options) =>
            ChangeStateAsync(OperationState.Blocked, OperationState.InProgress, options);

        public Task CancelAsync() =>
            ChangeStateAsync(State, OperationState.Cancelling);

        private async Task StartAsync()
        {
            var operations = _groupedOperationsToExecute.Select(g => g.Operations).ToArray();

            await ExecuteOperationsAsync(operations);
        }

        private async Task UnblockAsync(OperationContinuationOptions options)
        {

        }

        private async Task PauseAsync()
        {

        }

        private async Task UnpauseAsync()
        {

        }

        private async Task StopAsync()
        {
            _cancellationTokenSource.Cancel();
            // TODO: wait?

            var cancelOperations = _groupedOperationsToExecute
                .Reverse()
                .Where((o, i) => o.Operations[i].State.IsCancellationAvailable())
                .Select(g => g.CancelOperations)
                .ToArray();

            await ExecuteOperationsAsync(cancelOperations);
        }

        private async Task ExecuteOperationsAsync(
            IReadOnlyList<IReadOnlyList<IInternalOperation>> groupedOperationsToExecute)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            _totalOperationsCount = groupedOperationsToExecute.Sum(g => g.Count);

            foreach (var operationsGroup in groupedOperationsToExecute)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _taskCompletionSource = new TaskCompletionSource<bool>();

                _finishedOperationsCount = 0;
                _currentOperationsGroup = operationsGroup;

                for (var i = 0; i < _currentOperationsGroup.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var currentOperation = operationsGroup[i];
                    SubscribeToEvents(currentOperation);

                    await _taskPool.ExecuteAsync(() => currentOperation.RunAsync(cancellationToken));
                }

                await _taskCompletionSource.Task;
            }
        }

        private void CurrentOperationOnStateChanged(object sender, OperationStateChangedEventArgs e)
        {
            if (e.OperationState.IsCompleted())
            {
                var operation = (IInternalOperation) sender;
                UnsubscribeFromEvents(operation);

                var finishedOperationsCount = Interlocked.Increment(ref _finishedOperationsCount);
                if (finishedOperationsCount == _currentOperationsGroup.Count)
                {
                    _taskCompletionSource.SetResult(true);
                }
            }

            UpdateProgress();
        }

        private void SubscribeToEvents(IInternalOperation currentOperation)
        {
            currentOperation.StateChanged += CurrentOperationOnStateChanged;
            currentOperation.ProgressChanged += CurrentOperationOnProgressChanged;
        }


        private void UnsubscribeFromEvents(IInternalOperation currentOperation)
        {
            currentOperation.StateChanged -= CurrentOperationOnStateChanged;
            currentOperation.ProgressChanged += CurrentOperationOnProgressChanged;
        }

        private void CurrentOperationOnProgressChanged(object sender, OperationProgressChangedEventArgs e) =>
            UpdateProgress();

        private void UpdateProgress()
        {
            // TODO: prev group?
            CurrentProgress = _currentOperationsGroup.Sum(o => o.CurrentProgress) / _totalOperationsCount;
        }

        private async Task ChangeStateAsync(
            OperationState expectedState,
            OperationState requestedState,
            OperationContinuationOptions options = null)
        {
            var task = (State, requestedState) switch
            {
                _ when State != expectedState =>
                    throw new InvalidOperationException($"Inner state {State} is not {expectedState}"),

                (OperationState.NotStarted, OperationState.InProgress) =>
                    WrapAsync(StartAsync, OperationState.InProgress, OperationState.Finished),

                _ when State.IsCancellationAvailable() && requestedState is OperationState.Cancelling =>
                    WrapAsync(StopAsync, OperationState.Cancelling, OperationState.Cancelled),

                (OperationState.InProgress, OperationState.Failed) => Task.CompletedTask, // TODO: cleanup?

                // (OperationState.InProgress, OperationState.Paused) =>
                //     WrapAsync(PauseAsync, OperationState.P, OperationState.Cancelled),

                (OperationState.Blocked, OperationState.InProgress) when options is null =>
                    throw new ArgumentNullException(nameof(options)),

                (OperationState.Blocked, OperationState.InProgress) =>
                    WrapAsync(() => UnblockAsync(options), OperationState.InProgress, OperationState.Finished),

                (OperationState.Paused, OperationState.InProgress) =>
                    WrapAsync(UnpauseAsync, OperationState.InProgress, OperationState.Finished),

                (OperationState.Cancelling, OperationState.Cancelled) => Task.CompletedTask,
                (OperationState.InProgress, OperationState.Finished) => Task.CompletedTask,
                (OperationState.InProgress, OperationState.Blocked) => Task.CompletedTask,

                _ => throw new InvalidOperationException($"{State} has no transition to {requestedState}")
            };

            State = requestedState;
            await task;
        }

        private Task WrapAsync(Func<Task> taskFactory, OperationState expected, OperationState requested) =>
            taskFactory().ContinueWith(t => ChangeStateAsync(expected, requested));

    }
}