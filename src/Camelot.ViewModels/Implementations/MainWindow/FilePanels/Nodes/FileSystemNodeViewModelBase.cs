using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;

public abstract class FileSystemNodeViewModelBase : ViewModelBase, IFileSystemNodeViewModel
{
    private readonly IFileSystemNodeOpeningBehavior _fileSystemNodeOpeningBehavior;
    private readonly IFileSystemNodePropertiesBehavior _fileSystemNodePropertiesBehavior;
    private readonly IFileSystemNodeFacade _fileSystemNodeFacade;

    public DateTime LastModifiedDateTime { get; set; }

    public string FullPath { get; set; }

    public string Name { get; set; }

    public string FullName { get; set; }

    [Reactive]
    public bool IsEditing { get; set; }

    public bool IsArchive => _fileSystemNodeFacade.CheckIfNodeIsArchive(FullPath);

    public bool ShouldShowOpenSubmenu { get; }

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
        IFileSystemNodePropertiesBehavior fileSystemNodePropertiesBehavior,
        IFileSystemNodeFacade fileSystemNodeFacade,
        bool shouldShowOpenSubmenu)
    {
        _fileSystemNodeOpeningBehavior = fileSystemNodeOpeningBehavior;
        _fileSystemNodePropertiesBehavior = fileSystemNodePropertiesBehavior;
        _fileSystemNodeFacade = fileSystemNodeFacade;

        ShouldShowOpenSubmenu = shouldShowOpenSubmenu;

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

    private Task PackAsync() => _fileSystemNodeFacade.PackAsync(FullPath);

    private Task OpenWithAsync() => _fileSystemNodeFacade.OpenWithAsync(_fileSystemNodeOpeningBehavior, FullPath);

    private Task ExtractAsync(ExtractCommandType commandType) => _fileSystemNodeFacade.ExtractAsync(commandType, FullPath);

    private void Rename() => IsEditing = !_fileSystemNodeFacade.Rename(FullName, FullPath);

    private Task RenameInDialogAsync() => _fileSystemNodeFacade.RenameInDialogAsync(FullPath);

    private Task CopyToClipboardAsync() => _fileSystemNodeFacade.CopyToClipboardAsync(FullPath);

    private Task DeleteAsync() => _fileSystemNodeFacade.DeleteAsync(FullPath);

    private Task CopyAsync() => _fileSystemNodeFacade.CopyAsync(FullPath);

    private Task MoveAsync() => _fileSystemNodeFacade.MoveAsync(FullPath);

    private Task ShowPropertiesAsync() => _fileSystemNodePropertiesBehavior.ShowPropertiesAsync(FullPath);
}