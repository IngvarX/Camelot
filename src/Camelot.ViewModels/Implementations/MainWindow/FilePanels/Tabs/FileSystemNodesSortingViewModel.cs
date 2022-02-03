using System;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Tabs;

public class FileSystemNodesSortingViewModel : ViewModelBase, IFileSystemNodesSortingViewModel
{
    [Reactive]
    public SortingMode SortingColumn { get; set; }

    [Reactive]
    public bool IsSortingByAscendingEnabled { get; private set; }

    public event EventHandler<EventArgs> SortingSettingsChanged;

    public FileSystemNodesSortingViewModel(
        SortingMode sortingColumn,
        bool isSortingByAscendingEnabled)
    {
        SortingColumn = sortingColumn;
        IsSortingByAscendingEnabled = isSortingByAscendingEnabled;

        this.WhenAnyValue(vm => vm.SortingColumn, vm => vm.IsSortingByAscendingEnabled)
            .Subscribe(_ => FireSortingSettingsChangedEvent());
    }

    public void ToggleSortingDirection() => IsSortingByAscendingEnabled = !IsSortingByAscendingEnabled;

    private void FireSortingSettingsChangedEvent() => SortingSettingsChanged.Raise(this, EventArgs.Empty);
}