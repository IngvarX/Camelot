using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public abstract class FileSystemNodeViewModelBase : ViewModelBase, IFileSystemNodeViewModel
    {
        private readonly IFileSystemNodeOpeningBehavior _fileSystemNodeOpeningBehavior;
        private readonly IOperationsService _operationsService;
        private readonly IClipboardOperationsService _clipboardOperationsService;
        private readonly IFilesOperationsMediator _filesOperationsMediator;
        private readonly IFileSystemNodePropertiesBehavior _fileSystemNodePropertiesBehavior;
        private readonly IDialogService _dialogService;
        private readonly ITrashCanService _trashCanService;
        private readonly IArchiveService _archiveService;
        private readonly ISystemDialogService _systemDialogService;

        private IReadOnlyList<string> Files => new[] {FullPath};

        public DateTime LastModifiedDateTime { get; set; }

        public string FullPath { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        [Reactive]
        public bool IsEditing { get; set; }

        public bool IsArchive => _archiveService.CheckIfNodeIsArchive(FullPath);

        public bool IsWaitingForEdit { get; set; }

        public ICommand OpenCommand { get; }

        public ICommand OpenWithCommand { get; }

        public ICommand PackCommand { get; }

        public ICommand ExtractCommand { get; }

        public ICommand StartRenamingCommand { get; }

        public ICommand RenameCommand { get; }

        public ICommand CopyToClipboardCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand MoveCommand { get; }

        public ICommand ShowPropertiesCommand { get; }

        protected FileSystemNodeViewModelBase(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
            IOperationsService operationsService,
            IClipboardOperationsService clipboardOperationsService,
            IFilesOperationsMediator filesOperationsMediator,
            IFileSystemNodePropertiesBehavior fileSystemNodePropertiesBehavior,
            IDialogService dialogService,
            ITrashCanService trashCanService,
            IArchiveService archiveService,
            ISystemDialogService systemDialogService)
        {
            _fileSystemNodeOpeningBehavior = fileSystemNodeOpeningBehavior;
            _operationsService = operationsService;
            _clipboardOperationsService = clipboardOperationsService;
            _filesOperationsMediator = filesOperationsMediator;
            _fileSystemNodePropertiesBehavior = fileSystemNodePropertiesBehavior;
            _dialogService = dialogService;
            _trashCanService = trashCanService;
            _archiveService = archiveService;
            _systemDialogService = systemDialogService;

            OpenCommand = ReactiveCommand.Create(Open);
            OpenWithCommand = ReactiveCommand.Create(OpenWithAsync);
            PackCommand = ReactiveCommand.CreateFromTask(PackAsync);
            ExtractCommand = ReactiveCommand.CreateFromTask<ExtractCommandType>(ExtractAsync);
            StartRenamingCommand = ReactiveCommand.Create(StartRenaming);
            RenameCommand = ReactiveCommand.Create(Rename);
            CopyToClipboardCommand = ReactiveCommand.CreateFromTask(CopyToClipboardAsync);
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync);
            CopyCommand = ReactiveCommand.CreateFromTask(CopyAsync);
            MoveCommand = ReactiveCommand.CreateFromTask(MoveAsync);
            ShowPropertiesCommand = ReactiveCommand.CreateFromTask(ShowPropertiesAsync);
        }

        private void Open() => _fileSystemNodeOpeningBehavior.Open(FullPath);

        private async Task PackAsync()
        {
            var parameter = new CreateArchiveNavigationParameter(FullPath, true);
            var dialogResult = await _dialogService.ShowDialogAsync<CreateArchiveDialogResult, CreateArchiveNavigationParameter>(
                nameof(CreateArchiveDialogViewModel), parameter);
            if (dialogResult is null)
            {
                return;
            }

            await _archiveService.PackAsync(Files, dialogResult.ArchivePath, dialogResult.ArchiveType);
        }
        
        private async Task OpenWithAsync()
        {
            var extension = Path.GetExtension(FullName);
            await _dialogService.ShowDialogAsync(nameof(OpenWithDialogViewModel), new OpenWithNavigationParameter(extension));
        }

        private async Task ExtractAsync(ExtractCommandType commandType)
        {
            if (!IsArchive)
            {
                return;
            }

            switch (commandType)
            {
                case ExtractCommandType.CurrentDirectory:
                    await _archiveService.ExtractAsync(FullPath);
                    break;
                case ExtractCommandType.NewDirectory:
                    await _archiveService.ExtractToNewDirectoryAsync(FullPath);
                    break;
                case ExtractCommandType.SelectDirectory:
                    var directory = await _systemDialogService.GetDirectoryAsync();
                    if (!string.IsNullOrWhiteSpace(directory))
                    {
                        await _archiveService.ExtractAsync(FullPath, directory);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
            }
        }

        private void StartRenaming() => IsEditing = true;

        private void Rename()
        {
            if (string.IsNullOrEmpty(FullName))
            {
                return;
            }

            var renameResult = _operationsService.Rename(FullPath, FullName);
            if (renameResult)
            {
                IsEditing = false;
            }
        }

        private Task CopyToClipboardAsync() => _clipboardOperationsService.CopyFilesAsync(Files);

        private async Task DeleteAsync()
        {
            var result = await ShowRemoveConfirmationDialogAsync();
            if (result)
            {
                await _trashCanService.MoveToTrashAsync(Files);
            }
        }

        private Task CopyAsync() => _operationsService.CopyAsync(Files, _filesOperationsMediator.OutputDirectory);

        private Task MoveAsync() => _operationsService.MoveAsync(Files, _filesOperationsMediator.OutputDirectory);

        private Task ShowPropertiesAsync() => _fileSystemNodePropertiesBehavior.ShowPropertiesAsync(FullPath);

        private async Task<bool> ShowRemoveConfirmationDialogAsync()
        {
            var navigationParameter = new NodesRemovingNavigationParameter(Files);
            var result = await _dialogService
                .ShowDialogAsync<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>(
                    nameof(RemoveNodesConfirmationDialogViewModel), navigationParameter);

            return result?.IsConfirmed ?? false;
        }
    }
}
