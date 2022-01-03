using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Services.Implementations;

public class FileSystemNodeFacade : IFileSystemNodeFacade
{
    private readonly IOperationsService _operationsService;
    private readonly IClipboardOperationsService _clipboardOperationsService;
    private readonly IFilesOperationsMediator _filesOperationsMediator;
    private readonly IDialogService _dialogService;
    private readonly ITrashCanService _trashCanService;
    private readonly IArchiveService _archiveService;
    private readonly ISystemDialogService _systemDialogService;
    private readonly IOpenWithApplicationService _openWithApplicationService;
    private readonly IPathService _pathService;

    public FileSystemNodeFacade(
        IOperationsService operationsService,
        IClipboardOperationsService clipboardOperationsService,
        IFilesOperationsMediator filesOperationsMediator,
        IDialogService dialogService,
        ITrashCanService trashCanService,
        IArchiveService archiveService,
        ISystemDialogService systemDialogService,
        IOpenWithApplicationService openWithApplicationService,
        IPathService pathService)
    {
        _operationsService = operationsService;
        _clipboardOperationsService = clipboardOperationsService;
        _filesOperationsMediator = filesOperationsMediator;
        _dialogService = dialogService;
        _trashCanService = trashCanService;
        _archiveService = archiveService;
        _systemDialogService = systemDialogService;
        _openWithApplicationService = openWithApplicationService;
        _pathService = pathService;
    }

    public async Task PackAsync(string fullPath)
    {
        var dialogResult = await ShowPackDialogAsync(fullPath);
        if (dialogResult is not null)
        {
            await _archiveService.PackAsync(CreateFilesList(fullPath), dialogResult.ArchivePath,
                dialogResult.ArchiveType);
        }
    }

    public async Task OpenWithAsync(IFileSystemNodeOpeningBehavior behavior, string fullPath)
    {
        var dialogResult = await ShowOpenWithDialogAsync(fullPath);
        if (dialogResult is null)
        {
            return;
        }

        behavior.OpenWith(dialogResult.Application.ExecutePath,
            dialogResult.Application.Arguments, fullPath);

        if (dialogResult.IsDefaultApplication)
        {
            _openWithApplicationService.SaveSelectedApplication(dialogResult.FileExtension, dialogResult.Application);
        }
    }

    public async Task ExtractAsync(ExtractCommandType commandType, string fullPath)
    {
        if (!CheckIfNodeIsArchive(fullPath))
        {
            return;
        }

        switch (commandType)
        {
            case ExtractCommandType.CurrentDirectory:
                await _archiveService.ExtractAsync(fullPath);
                break;
            case ExtractCommandType.NewDirectory:
                await _archiveService.ExtractToNewDirectoryAsync(fullPath);
                break;
            case ExtractCommandType.SelectDirectory:
                var directory = await _systemDialogService.GetDirectoryAsync();
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    await _archiveService.ExtractAsync(fullPath, directory);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
        }
    }

    public bool Rename(string fullName, string fullPath) =>
        !string.IsNullOrEmpty(fullName) && _operationsService.Rename(fullPath, fullName);

    public async Task RenameInDialogAsync(string fullPath)
    {
        var newPath = await ShowRenameDialogAsync(fullPath);
        if (newPath is not null)
        {
            _operationsService.Rename(fullPath, newPath);
        }
    }

    public Task CopyToClipboardAsync(string filePath) =>
        _clipboardOperationsService.CopyFilesAsync(CreateFilesList(filePath));

    public async Task DeleteAsync(string filePath)
    {
        var files = CreateFilesList(filePath);
        var result = await ShowRemoveConfirmationDialogAsync(files);
        if (result)
        {
            await _trashCanService.MoveToTrashAsync(files);
        }
    }

    public Task CopyAsync(string filePath) =>
        _operationsService.CopyAsync(CreateFilesList(filePath), _filesOperationsMediator.OutputDirectory);

    public Task MoveAsync(string filePath) =>
        _operationsService.MoveAsync(CreateFilesList(filePath), _filesOperationsMediator.OutputDirectory);

    public bool CheckIfNodeIsArchive(string filePath) => _archiveService.CheckIfNodeIsArchive(filePath);

    private async Task<CreateArchiveDialogResult> ShowPackDialogAsync(string fullPath)
    {
        var parameter = new CreateArchiveNavigationParameter(fullPath, true);

        return await _dialogService.ShowDialogAsync<CreateArchiveDialogResult, CreateArchiveNavigationParameter>(
            nameof(CreateArchiveDialogViewModel), parameter);
    }

    private async Task<OpenWithDialogResult> ShowOpenWithDialogAsync(string fullPath)
    {
        var fileExtension = _pathService.GetExtension(fullPath);
        var selectedApplication = _openWithApplicationService.GetSelectedApplication(fileExtension);
        var parameter = new OpenWithNavigationParameter(fileExtension, selectedApplication);

        return await _dialogService.ShowDialogAsync<OpenWithDialogResult, OpenWithNavigationParameter>(
            nameof(OpenWithDialogViewModel), parameter);
    }

    private async Task<string> ShowRenameDialogAsync(string fullPath)
    {
        var navigationParameter = new RenameNodeNavigationParameter(fullPath);
        var result = await _dialogService
            .ShowDialogAsync<RenameNodeDialogResult, RenameNodeNavigationParameter>(
                nameof(RenameNodeDialogViewModel), navigationParameter);

        return result?.NodeName;
    }

    private async Task<bool> ShowRemoveConfirmationDialogAsync(IReadOnlyList<string> files)
    {
        var navigationParameter = new NodesRemovingNavigationParameter(files);
        var result = await _dialogService
            .ShowDialogAsync<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>(
                nameof(RemoveNodesConfirmationDialogViewModel), navigationParameter);

        return result?.IsConfirmed ?? false;
    }

    private static IReadOnlyList<string> CreateFilesList(string filePath) => new[] {filePath};
}