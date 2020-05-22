using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
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
        private readonly ITrashCanService _trashCanService;

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
            ITrashCanService trashCanService)
        {
            _filesOperationsMediator = filesOperationsMediator;
            _operationsService = operationsService;
            _filesSelectionService = filesSelectionService;
            _dialogService = dialogService;
            _directoryService = directoryService;
            _trashCanService = trashCanService;

            OpenCommand = ReactiveCommand.Create(Open);
            OpenInDefaultEditorCommand = ReactiveCommand.Create(OpenInDefaultEditor);
            CopyCommand = ReactiveCommand.Create(() => Task.Run(CopyAsync));
            MoveCommand = ReactiveCommand.Create(() => Task.Run(MoveAsync));
            NewDirectoryCommand = ReactiveCommand.CreateFromTask(CreateNewDirectoryAsync);
            RemoveCommand = ReactiveCommand.CreateFromTask(RemoveAsync);
            RemoveToTrashCommand = ReactiveCommand.CreateFromTask(RemoveToTrashAsync);
        }

        private void Open() => _filesOperationsMediator.ActiveFilesPanelViewModel.OpenLastSelectedFile();

        private void OpenInDefaultEditor() => _operationsService.OpenFiles(GetSelectedFiles());

        private Task CopyAsync() => _operationsService.CopyAsync(GetSelectedFiles(),
            _filesOperationsMediator.OutputDirectory);

        private Task MoveAsync() => _operationsService.MoveAsync(GetSelectedFiles(),
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
            var filesToRemove = GetSelectedFiles();
            if (!filesToRemove.Any())
            {
                return;
            }

            var navigationParameter = new NodesRemovingNavigationParameter(filesToRemove, false);
            var isConfirmed = await ShowRemoveConfirmationDialogAsync(navigationParameter);
            if (isConfirmed)
            {
                 _operationsService.RemoveAsync(filesToRemove).Forget();
            }
        }

        private async Task RemoveToTrashAsync()
        {
            var filesToRemove = GetSelectedFiles();
            if (!filesToRemove.Any())
            {
                return;
            }

            var navigationParameter = new NodesRemovingNavigationParameter(filesToRemove, true);
            var isConfirmed = await ShowRemoveConfirmationDialogAsync(navigationParameter);
            if (isConfirmed)
            {
                _trashCanService.MoveToTrashAsync(filesToRemove).Forget();
            }
        }

        private Task<bool> ShowRemoveConfirmationDialogAsync(NodesRemovingNavigationParameter navigationParameter) =>
            _dialogService.ShowDialogAsync<bool, NodesRemovingNavigationParameter>(
                nameof(RemoveNodesConfirmationDialogViewModel), navigationParameter);

        private IReadOnlyList<string> GetSelectedFiles() =>
            _filesSelectionService
                .SelectedFiles
                .ToArray();
    }
}