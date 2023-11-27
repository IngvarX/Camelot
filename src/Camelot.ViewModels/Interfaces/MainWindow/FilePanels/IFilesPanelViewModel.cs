using System;
using System.Collections.Generic;
using System.Windows.Input;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.EventArgs;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.Operations;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

public interface IFilesPanelViewModel
{
    ITabsListViewModel TabsListViewModel { get; }

    ISearchViewModel SearchViewModel { get; }

    IQuickSearchViewModel QuickSearchViewModel { get; }

    IOperationsViewModel OperationsViewModel { get; }

    IDirectorySelectorViewModel DirectorySelectorViewModel { get; }

    IDragAndDropOperationsMediator DragAndDropOperationsMediator { get; }

    IClipboardOperationsViewModel ClipboardOperationsViewModel { get; }

    IList<IFileSystemNodeViewModel> SelectedFileSystemNodes { get; }

    IEnumerable<IFileSystemNodeViewModel> FileSystemNodes { get; }

    bool IsActive { get; }

    string CurrentDirectory { get; set; }

    event EventHandler<System.EventArgs> Activated;

    event EventHandler<System.EventArgs> Deactivated;

    event EventHandler<System.EventArgs> CurrentDirectoryChanged;

    event EventHandler<SelectionAddedEventArgs> SelectionAdded;

    event EventHandler<SelectionRemovedEventArgs> SelectionRemoved;

    ICommand ActivateCommand { get; }

    void Activate();

    void Deactivate();
}