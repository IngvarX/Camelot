using System;
using System.Linq;
using Avalonia;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Interfaces.MainWindow;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public class OperationsStateViewModel : ViewModelBase, IOperationsStateViewModel
    {
        private readonly IOperationsStateService _operationsStateService;

        private int _totalProgress;

        public int TotalProgress
        {
            get => _totalProgress;
            set => this.RaiseAndSetIfChanged(ref _totalProgress, value);
        }

        public OperationsStateViewModel(
            IOperationsStateService operationsStateService)
        {
            _operationsStateService = operationsStateService;

            SubscribeToEvents();

            TotalProgress = 50;
        }

        private void SubscribeToEvents()
        {
            _operationsStateService.OperationStarted += OperationsStateServiceOnOperationStarted;
        }

        private void OperationsStateServiceOnOperationStarted(object sender, OperationStartedEventArgs e)
        {
            SubscribeToEvents(e.Operation);
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
            if (e.OperationState == OperationState.Finished)
            {
                UnsubscribeFromEvents(operation);
            }

            // TODO: change status
        }

        private void OperationOnProgressChanged(object sender, OperationProgressChangedEventArgs e)
        {
            TotalProgress = 30;

            var activeOperations = GetActiveOperations();
            if (!activeOperations.Any())
            {
                return;
            }

            var averageProgress = activeOperations.Average(o => o.CurrentProgress);
            averageProgress = 0.75;
            TotalProgress = (int) (averageProgress * 100);
        }

        private IOperation[] GetActiveOperations() =>
            _operationsStateService.ActiveOperations.ToArray();
    }
}