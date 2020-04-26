using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Services.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.NavigationParameters;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public class OperationsViewModel : ViewModelBase, IOperationsViewModel
    {
        private readonly IFilesOperationsMediator _filesOperationsMediator;
        private readonly IOperationsService _operationsService;
        private readonly IFilesSelectionService _filesSelectionService;
        private readonly IDialogService _dialogService;
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;

        public ICommand EditCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand MoveCommand { get; }

        public ICommand NewDirectoryCommand { get; }

        public ICommand RemoveCommand { get; }

        public OperationsViewModel(
            IFilesOperationsMediator filesOperationsMediator,
            IOperationsService operationsService,
            IFilesSelectionService filesSelectionService,
            IDialogService dialogService,
            IDirectoryService directoryService,
            IPathService pathService)
        {
            _filesOperationsMediator = filesOperationsMediator;
            _operationsService = operationsService;
            _filesSelectionService = filesSelectionService;
            _dialogService = dialogService;
            _directoryService = directoryService;
            _pathService = pathService;

            EditCommand = ReactiveCommand.Create(Edit);
            CopyCommand = ReactiveCommand.CreateFromTask(CopyAsync);
            MoveCommand = ReactiveCommand.CreateFromTask(MoveAsync);
            NewDirectoryCommand = ReactiveCommand.CreateFromTask(CreateNewDirectoryAsync);
            RemoveCommand = ReactiveCommand.CreateFromTask(RemoveAsync);
        }

        private void Edit() => _operationsService.EditFiles(_filesSelectionService.SelectedFiles);

        private Task CopyAsync() => _operationsService.CopyFilesAsync(_filesSelectionService.SelectedFiles,
            _filesOperationsMediator.OutputDirectory);

        private Task MoveAsync() => _operationsService.MoveFilesAsync(_filesSelectionService.SelectedFiles,
            _filesOperationsMediator.OutputDirectory);

        private async Task CreateNewDirectoryAsync()
        {
            var directoryName = await _dialogService.ShowDialogAsync<string>(nameof(CreateDirectoryDialogViewModel));
            if (!string.IsNullOrEmpty(directoryName))
            {
                _operationsService.CreateDirectory(_directoryService.SelectedDirectory, directoryName);
            }
        }

        private async Task RemoveAsync()
        {
            var filesToRemove = _filesSelectionService
                .SelectedFiles
                .Select(_pathService.GetFileName)
                .ToArray();
            if (!filesToRemove.Any())
            {
                return;
            }
            
            var navigationParameter = new NodesRemovingNavigationParameter(filesToRemove);
            var isConfirmed = await _dialogService.ShowDialogAsync<bool, NodesRemovingNavigationParameter>(
                nameof(RemoveNodesConfirmationDialogViewModel), navigationParameter);
            if (isConfirmed)
            {
                await _operationsService.RemoveFilesAsync(_filesSelectionService.SelectedFiles);
            }
        }
    }
}