using System;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Services.Interfaces;

public interface IFilesOperationsMediator
{
    IFilesPanelViewModel ActiveFilesPanelViewModel { get; }

    IFilesPanelViewModel InactiveFilesPanelViewModel { get; }

    string OutputDirectory { get; }

    event EventHandler<EventArgs> ActiveFilesPanelChanged;

    void Register(IFilesPanelViewModel activeFilesPanelViewModel, IFilesPanelViewModel inactiveFilesPanelViewModel);

    void ToggleSearchPanelVisibility();
}