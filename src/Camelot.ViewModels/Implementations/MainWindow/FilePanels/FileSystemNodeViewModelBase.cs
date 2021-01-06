using System;
using System.Collections.Generic;
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
        private readonly IOpenWithApplicationService _openWithApplicationService;
        private readonly IPathService _pathService;

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

        public ICommand RenameCommand { get; }

        public ICommand RenameInDialogCommand { get; }

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
            ISystemDialogService systemDialogService,
            IOpenWithApplicationService openWithApplicationService,
            IPathService pathService)
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
            _openWithApplicationService = openWithApplicationService;
            _pathService = pathService;

            OpenCommand = ReactiveCommand.Create(Open);
            OpenWithCommand = ReactiveCommand.Create(OpenWithAsync);
            PackCommand = ReactiveCommand.CreateFromTask(PackAsync);
            ExtractCommand = ReactiveCommand.CreateFromTask<ExtractCommandType>(ExtractAsync);
            RenameCommand = ReactiveCommand.Create(Rename);
            RenameInDialogCommand = ReactiveCommand.CreateFromTask(RenameInDialogAsync);
            CopyToClipboardCommand = ReactiveCommand.CreateFromTask(CopyToClipboardAsync);
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync);
            CopyCommand = ReactiveCommand.CreateFromTask(CopyAsync);
            MoveCommand = ReactiveCommand.CreateFromTask(MoveAsync);
            ShowPropertiesCommand = ReactiveCommand.CreateFromTask(ShowPropertiesAsync);
        }

        private void Open() => _fileSystemNodeOpeningBehavior.Open(FullPath);

        private async Task PackAsync()
        {
            var dialogResult = await ShowPackDialogAsync();
            if (dialogResult != null)
            {
                await _archiveService.PackAsync(Files, dialogResult.ArchivePath, dialogResult.ArchiveType);
            }
        }

        private async Task OpenWithAsync()
        {
            var dialogResult = await ShowOpenWithDialogAsync();
            if (dialogResult is null)
            {
                return;
            }

            _fileSystemNodeOpeningBehavior.OpenWith(dialogResult.Application.ExecutePath,
                dialogResult.Application.Arguments, FullPath);

            if (dialogResult.IsDefaultApplication)
            {
                _openWithApplicationService.SaveSelectedApplication(dialogResult.FileExtension, dialogResult.Application);
            }
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

        private async Task RenameInDialogAsync()
        {
            var newPath = await ShowRenameDialogAsync();
            if (newPath != null)
            {
                _operationsService.Rename(FullPath, newPath);
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

        private async Task<CreateArchiveDialogResult> ShowPackDialogAsync()
        {
            var parameter = new CreateArchiveNavigationParameter(FullPath, true);

            return await _dialogService.ShowDialogAsync<CreateArchiveDialogResult, CreateArchiveNavigationParameter>(
                nameof(CreateArchiveDialogViewModel), parameter);
        }

        private async Task<OpenWithDialogResult> ShowOpenWithDialogAsync()
        {
            var fileExtension = _pathService.GetExtension(FullName);
            var selectedApplication = _openWithApplicationService.GetSelectedApplication(fileExtension);
            var parameter = new OpenWithNavigationParameter(fileExtension, selectedApplication);

            return await _dialogService.ShowDialogAsync<OpenWithDialogResult, OpenWithNavigationParameter>(
                nameof(OpenWithDialogViewModel), parameter);
        }

        private async Task<bool> ShowRemoveConfirmationDialogAsync()
        {
            var navigationParameter = new NodesRemovingNavigationParameter(Files);
            var result = await _dialogService
                .ShowDialogAsync<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>(
                    nameof(RemoveNodesConfirmationDialogViewModel), navigationParameter);

            return result?.IsConfirmed ?? false;
        }

        private async Task<string> ShowRenameDialogAsync()
        {
            var navigationParameter = new RenameNodeNavigationParameter(FullPath);
            var result = await _dialogService
                .ShowDialogAsync<RenameNodeDialogResult, RenameNodeNavigationParameter>(
                    nameof(RenameNodeDialogViewModel), navigationParameter);

            return result?.NodeName;
        }
    }
}
