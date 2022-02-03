using System;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;

public interface IFileSystemNodesSortingViewModel
{
    SortingMode SortingColumn { get; set; }

    bool IsSortingByAscendingEnabled { get; }

    void ToggleSortingDirection();

    event EventHandler<EventArgs> SortingSettingsChanged;
}