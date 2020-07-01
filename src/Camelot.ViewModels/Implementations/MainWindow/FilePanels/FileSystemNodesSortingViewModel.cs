using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class FileSystemNodesSortingViewModel : ViewModelBase, IFileSystemNodesSortingViewModel
    {
        private SortingColumn _sortingColumn;
        private bool _isSortingByAscendingEnabled;

        public SortingColumn SortingColumn
        {
            get => _sortingColumn;
            set => this.RaiseAndSetIfChanged(ref _sortingColumn, value);
        }

        public bool IsSortingByAscendingEnabled
        {
            get => _isSortingByAscendingEnabled;
            private set => this.RaiseAndSetIfChanged(ref _isSortingByAscendingEnabled, value);
        }

        public FileSystemNodesSortingViewModel(
            SortingColumn sortingColumn,
            bool isSortingByAscendingEnabled)
        {
            _sortingColumn = sortingColumn;
            _isSortingByAscendingEnabled = isSortingByAscendingEnabled;
        }

        public void ToggleSortingDirection() => IsSortingByAscendingEnabled = !IsSortingByAscendingEnabled;
    }
}