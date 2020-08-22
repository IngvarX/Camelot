using System;
using System.Linq;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.OperationsStates
{
    public class OperationStateViewModel : ViewModelBase, IOperationStateViewModel
    {
        private readonly IPathService _pathService;
        private readonly IOperation _operation;

        public OperationType OperationType => _operation.Info.OperationType;

        [Reactive]
        public double Progress { get; set; }

        [Reactive]
        public OperationState State { get; set; }

        public int SourceFilesCount => _operation.Info.Files.Count;

        public int SourceDirectoriesCount => _operation.Info.Directories.Count;

        public bool IsProcessingSingleFile => SourceFilesCount + SourceDirectoriesCount == 1;

        public string SourceFile => IsProcessingSingleFile
            ? _pathService.GetFileName(_operation.Info.Directories.FirstOrDefault() ?? _operation.Info.Files.FirstOrDefault())
            : null;

        public string SourceDirectory => _pathService.GetFileName(_operation.Info.SourceDirectory);

        public string TargetDirectory => _pathService.GetFileName(_operation.Info.TargetDirectory);

        public ICommand CancelCommand { get; }

        public OperationStateViewModel(
            IPathService pathService,
            IOperation operation)
        {
            _pathService = pathService;
            _operation = operation;

            CancelCommand = ReactiveCommand.CreateFromTask(_operation.CancelAsync);

            SubscribeToEvents();
            State = operation.State;
        }

        private void SubscribeToEvents()
        {
            _operation.ProgressChanged += OperationOnProgressChanged;
            _operation.StateChanged += OperationOnStateChanged;
        }

        private void UnsubscribeFromEvents()
        {
            _operation.ProgressChanged -= OperationOnProgressChanged;
            _operation.StateChanged -= OperationOnStateChanged;
        }

        private void OperationOnProgressChanged(object sender, OperationProgressChangedEventArgs e)
        {
            Progress = e.CurrentProgress * 100;
        }

        private void OperationOnStateChanged(object sender, OperationStateChangedEventArgs e)
        {
            State = e.OperationState;
            switch (State)
            {
                case OperationState.NotStarted:
                case OperationState.InProgress:
                case OperationState.Blocked:
                case OperationState.Paused:
                case OperationState.Pausing:
                case OperationState.Unpausing:
                case OperationState.Cancelling:
                case OperationState.Skipped:
                    break;
                case OperationState.Finished:
                case OperationState.Cancelled:
                case OperationState.Failed:
                    UnsubscribeFromEvents();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(State), State, null);
            }
        }
    }
}