using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Interfaces.MainWindow;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public class OperationsStateViewModel : ViewModelBase, IOperationsStateViewModel
    {
        private readonly IFileOperationsStateService _fileOperationsStateService;

        private string _totalProgress;

        public string TotalProgress
        {
            get => _totalProgress;
            set => this.RaiseAndSetIfChanged(ref _totalProgress, value);
        }

        public OperationsStateViewModel(
            IFileOperationsStateService fileOperationsStateService)
        {
            _fileOperationsStateService = fileOperationsStateService;

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _fileOperationsStateService.OperationStarted += FileOperationsStateServiceOnOperationStarted;
        }

        private void FileOperationsStateServiceOnOperationStarted(object sender, OperationStartedEventArgs e)
        {
            SubscribeToEvents(e.Operation);
        }

        private void SubscribeToEvents(IOperation operation)
        {
            operation.StateChanged += OperationOnStateChanged;
            operation.ProgressChanged += OperationOnProgressChanged;
        }

        private void OperationOnStateChanged(object sender, OperationStateChangedEventArgs e)
        {

        }

        private void OperationOnProgressChanged(object sender, OperationProgressChangedEventArgs e)
        {

        }
    }
}