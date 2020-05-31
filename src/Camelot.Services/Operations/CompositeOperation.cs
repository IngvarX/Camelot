using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Extensions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.TaskPool.Interfaces;

namespace Camelot.Services.Operations
{
    public class CompositeOperation : OperationBase, ICompositeOperation
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

        public async Task RunAsync()
        {
            var operations = _groupedOperationsToExecute.Select(g => g.Operations).ToArray();

            await ExecuteOperationsAsync(operations);
        }

        public async Task ContinueAsync(OperationContinuationOptions options)
        {

        }

        public async Task CancelAsync()
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
    }
}