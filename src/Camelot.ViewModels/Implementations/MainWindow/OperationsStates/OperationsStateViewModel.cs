using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.OperationsStates
{
    public class OperationsStateViewModel : ViewModelBase, IOperationsStateViewModel
    {
        private readonly IOperationsStateService _operationsStateService;

        private readonly ObservableCollection<IOperationViewModel> _operations;
        private readonly IDictionary<IOperation, IOperationViewModel> _operationsViewModelsDictionary;

        private int _totalProgress;

        public int TotalProgress
        {
            get => _totalProgress;
            set => this.RaiseAndSetIfChanged(ref _totalProgress, value);
        }

        public bool ShouldShowTotalProgress => TotalProgress > 0 && TotalProgress < 100;

        public IEnumerable<IOperationViewModel> Operations => _operations;

        public OperationsStateViewModel(
            IOperationsStateService operationsStateService)
        {
            _operationsStateService = operationsStateService;

            _operations = new ObservableCollection<IOperationViewModel>();
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
            _operations.Add(viewModel);
            _operationsViewModelsDictionary[operation] = viewModel;
        }

        private void RemoveOperation(IOperation operation)
        {
            UnsubscribeFromEvents(operation);

            var viewModel = _operationsViewModelsDictionary[operation];
            _operations.Remove(viewModel);
            _operationsViewModelsDictionary.Remove(operation);
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
                TotalProgress = 100;

                return;
            }

            var averageProgress = activeOperations.Average(o => o.CurrentProgress);
            TotalProgress = (int) (averageProgress * 100);
        }

        private IOperation[] GetActiveOperations() =>
            _operationsStateService.ActiveOperations.ToArray();

        private static IOperationViewModel CreateFrom(IOperation operation) =>
            new OperationViewModel(operation);
    }
}