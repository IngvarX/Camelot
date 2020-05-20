using System;
using System.Linq;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.OperationsStates
{
    public class OperationViewModel : ViewModelBase, IOperationViewModel
    {
        private readonly IPathService _pathService;
        private readonly IOperation _operation;

        private double _progress;
        private OperationState _state;

        public OperationType OperationType => _operation.Info.OperationType;

        public double Progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }

        public OperationState State
        {
            get => _state;
            set
            {
                this.RaiseAndSetIfChanged(ref _state, value);
                this.RaisePropertyChanged(nameof(IsInProgress));
            }
        }

        public int SourceFilesCount => _operation.Info.Files.Count;

        public int SourceDirectoriesCount => _operation.Info.Directories.Count;

        public bool IsProcessingSingleFile => SourceFilesCount + SourceDirectoriesCount == 1;

        public string SourceFile => IsProcessingSingleFile
            ? _pathService.GetFileName(_operation.Info.Directories.FirstOrDefault() ?? _operation.Info.Files.FirstOrDefault())
            : throw new InvalidOperationException();

        public bool IsInProgress => State == OperationState.InProgress;

        public bool IsSuccessful => State == OperationState.Finished;

        public string SourceDirectory => _pathService.GetFileName(_operation.Info.SourceDirectory);

        public string TargetDirectory => _pathService.GetFileName(_operation.Info.TargetDirectory);

        public ICommand CancelCommand { get; }

        public OperationViewModel(
            IPathService pathService,
            IOperation operation)
        {
            _pathService = pathService;
            _operation = operation;

            CancelCommand = ReactiveCommand.CreateFromTask(_operation.CancelAsync);

            SubscribeToEvents();
            State = operation.OperationState;
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
            var state = e.OperationState;
            State = state;
            switch (state)
            {
                case OperationState.NotStarted:
                case OperationState.InProgress:
                    break;
                case OperationState.Finished:
                case OperationState.Cancelled:
                case OperationState.Failed:
                    UnsubscribeFromEvents();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}