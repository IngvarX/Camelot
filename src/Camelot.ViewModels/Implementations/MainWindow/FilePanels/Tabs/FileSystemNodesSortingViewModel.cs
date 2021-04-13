using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Tabs
{
    public class FileSystemNodesSortingViewModel : ViewModelBase, IFileSystemNodesSortingViewModel
    {
        [Reactive]
        public SortingMode SortingColumn { get; set; }

        [Reactive]
        public bool IsSortingByAscendingEnabled { get; private set; }

        public FileSystemNodesSortingViewModel(
            SortingMode sortingColumn,
            bool isSortingByAscendingEnabled)
        {
            SortingColumn = sortingColumn;
            IsSortingByAscendingEnabled = isSortingByAscendingEnabled;
        }

        public void ToggleSortingDirection() => IsSortingByAscendingEnabled = !IsSortingByAscendingEnabled;
    }
}