using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Services.Abstractions;
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
        private readonly ITrashCanServiceFactory _trashCanServiceFactory;

        public ICommand OpenCommand { get; }
        
        public ICommand OpenInDefaultEditorCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand MoveCommand { get; }

        public ICommand NewDirectoryCommand { get; }

        public ICommand RemoveCommand { get; }
        
        public ICommand RemoveToTrashCommand { get; }
        
        public OperationsViewModel(
            IFilesOperationsMediator filesOperationsMediator,
            IOperationsService operationsService,
            IFilesSelectionService filesSelectionService,
            IDialogService dialogService,
            IDirectoryService directoryService,
            ITrashCanServiceFactory trashCanServiceFactory)
        {
            _filesOperationsMediator = filesOperationsMediator;
            _operationsService = operationsService;
            _filesSelectionService = filesSelectionService;
            _dialogService = dialogService;
            _directoryService = directoryService;
            _trashCanServiceFactory = trashCanServiceFactory;

            OpenCommand = ReactiveCommand.Create(Open);
            OpenInDefaultEditorCommand = ReactiveCommand.Create(OpenInDefaultEditor);
            CopyCommand = ReactiveCommand.CreateFromTask(CopyAsync);
            MoveCommand = ReactiveCommand.CreateFromTask(MoveAsync);
            NewDirectoryCommand = ReactiveCommand.CreateFromTask(CreateNewDirectoryAsync);
            RemoveCommand = ReactiveCommand.CreateFromTask(RemoveAsync);
            RemoveToTrashCommand = ReactiveCommand.CreateFromTask(RemoveToTrashAsync);
        }

        private void Open() => _filesOperationsMediator.ActiveFilesPanelViewModel.OpenLastSelectedFile();
        
        private void OpenInDefaultEditor() => _operationsService.OpenFiles(_filesSelectionService.SelectedFiles);

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
            var filesToRemove = GetFilesToRemove();
            if (!filesToRemove.Any())
            {
                return;
            }
            
            var navigationParameter = new NodesRemovingNavigationParameter(filesToRemove, false);
            var isConfirmed = await ShowRemoveConfirmationDialogAsync(navigationParameter);
            if (isConfirmed)
            {
                await _operationsService.RemoveFilesAsync(filesToRemove);
            }
        }
        
        private async Task RemoveToTrashAsync()
        {
            var filesToRemove = GetFilesToRemove();
            if (!filesToRemove.Any())
            {
                return;
            }

            var navigationParameter = new NodesRemovingNavigationParameter(filesToRemove, true);
            var isConfirmed = await ShowRemoveConfirmationDialogAsync(navigationParameter);
            if (isConfirmed)
            {
                var trashCanService = _trashCanServiceFactory.Create();
                
                await trashCanService.MoveToTrashAsync(filesToRemove);
            }
        }
        
        private Task<bool> ShowRemoveConfirmationDialogAsync(NodesRemovingNavigationParameter navigationParameter) =>
            _dialogService.ShowDialogAsync<bool, NodesRemovingNavigationParameter>(
                nameof(RemoveNodesConfirmationDialogViewModel), navigationParameter);

        private IReadOnlyCollection<string> GetFilesToRemove() =>
            _filesSelectionService
                .SelectedFiles
                .ToArray();
    }
}