using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public class OperationsViewModel : ViewModelBase, IOperationsViewModel
    {
        private readonly IFilesOperationsMediator _filesOperationsMediator;
        private readonly IOperationsService _operationsService;
        private readonly INodesSelectionService _nodesSelectionService;
        private readonly IDialogService _dialogService;
        private readonly IDirectoryService _directoryService;
        private readonly ITrashCanService _trashCanService;

        public ICommand OpenCommand { get; }

        public ICommand OpenInDefaultEditorCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand MoveCommand { get; }

        public ICommand CreateNewDirectoryCommand { get; }

        public ICommand CreateNewFileCommand { get; }

        public ICommand RemoveCommand { get; }

        public ICommand MoveToTrashCommand { get; }

        public OperationsViewModel(
            IFilesOperationsMediator filesOperationsMediator,
            IOperationsService operationsService,
            INodesSelectionService nodesSelectionService,
            IDialogService dialogService,
            IDirectoryService directoryService,
            ITrashCanService trashCanService)
        {
            _filesOperationsMediator = filesOperationsMediator;
            _operationsService = operationsService;
            _nodesSelectionService = nodesSelectionService;
            _dialogService = dialogService;
            _directoryService = directoryService;
            _trashCanService = trashCanService;

            OpenCommand = ReactiveCommand.Create(Open);
            OpenInDefaultEditorCommand = ReactiveCommand.Create(OpenInDefaultEditor);
            CopyCommand = ReactiveCommand.Create(Copy);
            MoveCommand = ReactiveCommand.Create(Move);
            CreateNewDirectoryCommand = ReactiveCommand.CreateFromTask(CreateNewDirectoryAsync);
            CreateNewFileCommand = ReactiveCommand.CreateFromTask(CreateNewFileAsync);
            RemoveCommand = ReactiveCommand.CreateFromTask(RemoveAsync);
            MoveToTrashCommand = ReactiveCommand.CreateFromTask(MoveToTrashAsync);
        }

        private void Open() => _filesOperationsMediator.ActiveFilesPanelViewModel.OpenLastSelectedFile();

        private void OpenInDefaultEditor() => _operationsService.OpenFiles(GetSelectedNodes());

        private void Copy() => Execute(() => _operationsService.CopyAsync(GetSelectedNodes(),
            _filesOperationsMediator.OutputDirectory));

        private void Move() => Execute(() => _operationsService.MoveAsync(GetSelectedNodes(),
            _filesOperationsMediator.OutputDirectory));

        private async Task CreateNewDirectoryAsync()
        {
            var parameter = new CreateNodeNavigationParameter(_directoryService.SelectedDirectory);
            var result = await _dialogService.ShowDialogAsync<CreateDirectoryDialogResult, CreateNodeNavigationParameter>(
                nameof(CreateDirectoryDialogViewModel), parameter);
            if (!string.IsNullOrEmpty(result?.DirectoryName))
            {
                _operationsService.CreateDirectory(_directoryService.SelectedDirectory, result.DirectoryName);
            }
        }

        private async Task CreateNewFileAsync()
        {
            var parameter = new CreateNodeNavigationParameter(_directoryService.SelectedDirectory);
            var result = await _dialogService.ShowDialogAsync<CreateFileDialogResult, CreateNodeNavigationParameter>(
                nameof(CreateFileDialogViewModel), parameter);
            if (!string.IsNullOrEmpty(result?.FileName))
            {
                _operationsService.CreateFile(_directoryService.SelectedDirectory, result.FileName);
            }
        }

        private async Task RemoveAsync()
        {
            var nodesToRemove = GetSelectedNodes();
            if (!nodesToRemove.Any())
            {
                return;
            }

            var navigationParameter = new NodesRemovingNavigationParameter(nodesToRemove, false);
            var result = await ShowRemoveConfirmationDialogAsync(navigationParameter);
            if (result)
            {
                Execute(() => _operationsService.RemoveAsync(nodesToRemove));
            }
        }

        private async Task MoveToTrashAsync()
        {
            var nodesToRemove = GetSelectedNodes();
            if (!nodesToRemove.Any())
            {
                return;
            }

            var navigationParameter = new NodesRemovingNavigationParameter(nodesToRemove);
            var result = await ShowRemoveConfirmationDialogAsync(navigationParameter);
            if (result)
            {
                Execute(() => _trashCanService.MoveToTrashAsync(nodesToRemove));
            }
        }

        private async Task<bool> ShowRemoveConfirmationDialogAsync(
            NodesRemovingNavigationParameter navigationParameter)
        {
            var result = await _dialogService
                .ShowDialogAsync<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>(
                    nameof(RemoveNodesConfirmationDialogViewModel), navigationParameter);

            return result?.IsConfirmed ?? false;
        }

        private IReadOnlyList<string> GetSelectedNodes() =>
            _nodesSelectionService
                .SelectedNodes
                .ToArray();

        private static void Execute(Action action) => Task.Factory.StartNew(action);
    }
}