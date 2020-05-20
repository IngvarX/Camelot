using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.OperationsStates
{
    public class OperationsStateViewModel : ViewModelBase, IOperationsStateViewModel
    {
        private const int MaximumFinishedOperationsCount = 10;

        private readonly IOperationsStateService _operationsStateService;
        private readonly IOperationViewModelFactory _operationViewModelFactory;

        private readonly ObservableCollection<IOperationViewModel> _activeOperations;
        private readonly Queue<IOperationViewModel> _finishedOperationsQueue;
        private readonly IDictionary<IOperation, IOperationViewModel> _operationsViewModelsDictionary;

        private int _totalProgress;

        public int TotalProgress
        {
            get => _totalProgress;
            set
            {
                this.RaiseAndSetIfChanged(ref _totalProgress, value);
                this.RaisePropertyChanged(nameof(IsInProgress));
            }
        }

        public bool IsInProgress => TotalProgress > 0 && TotalProgress < 100;

        public IEnumerable<IOperationViewModel> ActiveOperations => _activeOperations;

        public IEnumerable<IOperationViewModel> InactiveOperations => _finishedOperationsQueue.Reverse();

        public OperationsStateViewModel(
            IOperationsStateService operationsStateService,
            IOperationViewModelFactory operationViewModelFactory)
        {
            _operationsStateService = operationsStateService;
            _operationViewModelFactory = operationViewModelFactory;

            _activeOperations = new ObservableCollection<IOperationViewModel>();
            _finishedOperationsQueue = new Queue<IOperationViewModel>(MaximumFinishedOperationsCount);
            _operationsViewModelsDictionary = new ConcurrentDictionary<IOperation, IOperationViewModel>();

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _operationsStateService.OperationStarted += OperationsStateServiceOnOperationStarted;
        }

        private void OperationsStateServiceOnOperationStarted(object sender, OperationStartedEventArgs e)
        {
            AddOperation(e.Operation);
        }

        private void AddOperation(IOperation operation)
        {
            SubscribeToEvents(operation);

            var viewModel = CreateFrom(operation);
            _activeOperations.Add(viewModel);
            _operationsViewModelsDictionary[operation] = viewModel;
        }

        private void RemoveOperation(IOperation operation)
        {
            UnsubscribeFromEvents(operation);

            var viewModel = _operationsViewModelsDictionary[operation];
            _activeOperations.Remove(viewModel);
            _operationsViewModelsDictionary.Remove(operation);

            AddFinishedOperationViewModel(viewModel);
        }

        private void AddFinishedOperationViewModel(IOperationViewModel viewModel)
        {
            if (_finishedOperationsQueue.Count == MaximumFinishedOperationsCount)
            {
                _finishedOperationsQueue.Dequeue();
            }

            _finishedOperationsQueue.Enqueue(viewModel);
            this.RaisePropertyChanged(nameof(InactiveOperations));
        }

        private void SubscribeToEvents(IOperation operation)
        {
            operation.StateChanged += OperationOnStateChanged;
            operation.ProgressChanged += OperationOnProgressChanged;
        }

        private void UnsubscribeFromEvents(IOperation operation)
        {
            operation.StateChanged -= OperationOnStateChanged;
            operation.ProgressChanged -= OperationOnProgressChanged;
        }

        private void OperationOnStateChanged(object sender, OperationStateChangedEventArgs e)
        {
            var operation = (IOperation) sender;
            if (e.OperationState == OperationState.Finished || e.OperationState == OperationState.Cancelled)
            {
                RemoveOperation(operation);
            }

            // TODO: change status
        }

        private void OperationOnProgressChanged(object sender, OperationProgressChangedEventArgs e)
        {
            var activeOperations = GetActiveOperations();
            if (!activeOperations.Any())
            {
                TotalProgress = default;

                return;
            }

            var averageProgress = activeOperations.Average(o => o.CurrentProgress);
            TotalProgress = (int) (averageProgress * 100);
        }

        private IOperation[] GetActiveOperations() =>
            _operationsStateService.ActiveOperations.ToArray();

        private IOperationViewModel CreateFrom(IOperation operation) =>
            _operationViewModelFactory.Create(operation);
    }
}