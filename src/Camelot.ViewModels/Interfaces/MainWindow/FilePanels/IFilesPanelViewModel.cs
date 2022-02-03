using System;
using System.Collections.Generic;
using System.Windows.Input;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.Operations;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

public interface IFilesPanelViewModel
{
    ITabsListViewModel TabsListViewModel { get; }

    ISearchViewModel SearchViewModel { get; }

    IOperationsViewModel OperationsViewModel { get; }

    IDirectorySelectorViewModel DirectorySelectorViewModel { get; }

    IDragAndDropOperationsMediator DragAndDropOperationsMediator { get; }

    IClipboardOperationsViewModel ClipboardOperationsViewModel { get; }

    IList<IFileSystemNodeViewModel> SelectedFileSystemNodes { get; }

    bool IsActive { get; }

    string CurrentDirectory { get; set; }

    event EventHandler<EventArgs> Activated;

    event EventHandler<EventArgs> Deactivated;

    event EventHandler<EventArgs> CurrentDirectoryChanged;

    event EventHandler<SelectionAddedEventArgs> SelectionAdded;

    event EventHandler<SelectionRemovedEventArgs> SelectionRemoved;

    ICommand ActivateCommand { get; }

    void Activate();

    void Deactivate();
}