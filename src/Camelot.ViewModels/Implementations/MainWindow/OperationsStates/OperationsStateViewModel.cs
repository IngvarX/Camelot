using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;
using Camelot.Services.Abstractions.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.OperationsStates
{
    // TODO: rename
    public class OperationsStateViewModel : ViewModelBase, IOperationsStateViewModel
    {
        private const int MaximumFinishedOperationsCount = 10;

        private readonly IOperationsStateService _operationsStateService;
        private readonly IOperationStateViewModelFactory _operationStateViewModelFactory;
        private readonly IApplicationDispatcher _applicationDispatcher;
        private readonly IDialogService _dialogService;

        private readonly ObservableCollection<IOperationStateViewModel> _activeOperations;
        private readonly Queue<IOperationStateViewModel> _finishedOperationsQueue;
        private readonly IDictionary<IOperation, IOperationStateViewModel> _operationsViewModelsDictionary;

        private int _totalProgress;
        private bool _areAnyOperationsAvailable;

        public int TotalProgress
        {
            get => _totalProgress;
            set
            {
                this.RaiseAndSetIfChanged(ref _totalProgress, value);
                this.RaisePropertyChanged(nameof(IsInProgress));
            }
        }

        public bool AreAnyOperationsAvailable
        {
            get => _areAnyOperationsAvailable;
            set => this.RaiseAndSetIfChanged(ref _areAnyOperationsAvailable, value);
        }

        public bool IsInProgress => TotalProgress > 0 && TotalProgress < 100;

        public IEnumerable<IOperationStateViewModel> ActiveOperations => _activeOperations;

        public IEnumerable<IOperationStateViewModel> InactiveOperations => _finishedOperationsQueue.Reverse();

        public OperationsStateViewModel(
            IOperationsStateService operationsStateService,
            IOperationStateViewModelFactory operationStateViewModelFactory,
            IApplicationDispatcher applicationDispatcher,
            IDialogService dialogService)
        {
            _operationsStateService = operationsStateService;
            _operationStateViewModelFactory = operationStateViewModelFactory;
            _applicationDispatcher = applicationDispatcher;
            _dialogService = dialogService;

            _activeOperations = new ObservableCollection<IOperationStateViewModel>();
            _finishedOperationsQueue = new Queue<IOperationStateViewModel>(MaximumFinishedOperationsCount);
            _operationsViewModelsDictionary = new ConcurrentDictionary<IOperation, IOperationStateViewModel>();

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _operationsStateService.OperationStarted += OperationsStateServiceOnOperationStarted;
        }

        private void OperationsStateServiceOnOperationStarted(object sender, OperationStartedEventArgs e) =>
            _applicationDispatcher.Dispatch(() => AddOperation(e.Operation));

        private void AddOperation(IOperation operation)
        {
            SubscribeToEvents(operation);

            var viewModel = CreateFrom(operation);
            _activeOperations.Add(viewModel);
            _operationsViewModelsDictionary[operation] = viewModel;

            AreAnyOperationsAvailable = true;
            if (operation.State.IsCompleted())
            {
                RemoveOperation(operation);
            }
        }

        private void RemoveOperation(IOperation operation)
        {
            UnsubscribeFromEvents(operation);

            var isKeyAvailable = _operationsViewModelsDictionary.TryGetValue(operation, out var viewModel);
            if (!isKeyAvailable)
            {
                return;
            }

            _activeOperations.Remove(viewModel);
            _operationsViewModelsDictionary.Remove(operation);

            AddFinishedOperationViewModel(viewModel);
        }

        private void AddFinishedOperationViewModel(IOperationStateViewModel stateViewModel)
        {
            if (_finishedOperationsQueue.Count == MaximumFinishedOperationsCount)
            {
                _finishedOperationsQueue.Dequeue();
            }

            _finishedOperationsQueue.Enqueue(stateViewModel);
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

        private async void OperationOnStateChanged(object sender, OperationStateChangedEventArgs e)
        {
            var operation = (IOperation) sender;
            if (e.OperationState == OperationState.Finished || e.OperationState == OperationState.Cancelled)
            {
                _applicationDispatcher.Dispatch(() => RemoveOperation(operation));
            }

            if (e.OperationState == OperationState.Blocked)
            {
                await _applicationDispatcher.DispatchAsync(() => ProcessBlockedOperationAsync(operation));
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

        private async Task ProcessBlockedOperationAsync(IOperation operation)
        {
            var (sourceFilePath, destinationFilePath) = operation.BlockedFiles.Last();
            var navigationParameter = new OverwriteOptionsNavigationParameter(
                sourceFilePath, destinationFilePath);

            var dialogResult = await _dialogService.ShowDialogAsync<OverwriteOptionsDialogResult, OverwriteOptionsNavigationParameter>(
                nameof(OverwriteOptionsDialogViewModel), navigationParameter);

        }

        private IOperation[] GetActiveOperations() =>
            _operationsStateService.ActiveOperations.ToArray();

        private IOperationStateViewModel CreateFrom(IOperation operation) =>
            _operationStateViewModelFactory.Create(operation);
    }
}