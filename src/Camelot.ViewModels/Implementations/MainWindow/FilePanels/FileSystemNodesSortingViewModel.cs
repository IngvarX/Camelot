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
            set
            {
                this.RaiseAndSetIfChanged(ref _sortingColumn, value);
                this.RaisePropertyChanged(nameof(IsSortingByNameEnabled));
                this.RaisePropertyChanged(nameof(IsSortingByDateEnabled));
                this.RaisePropertyChanged(nameof(IsSortingByExtensionEnabled));
                this.RaisePropertyChanged(nameof(IsSortingBySizeEnabled));
            }
        }
        
        public bool IsSortingByAscendingEnabled
        {
            get => _isSortingByAscendingEnabled;
            private set => this.RaiseAndSetIfChanged(ref _isSortingByAscendingEnabled, value);
        }

        public bool IsSortingByNameEnabled => SortingColumn == SortingColumn.Name;
        
        public bool IsSortingByDateEnabled => SortingColumn == SortingColumn.Date;
        
        public bool IsSortingByExtensionEnabled => SortingColumn == SortingColumn.Extension;
        
        public bool IsSortingBySizeEnabled => SortingColumn == SortingColumn.Size;

        public FileSystemNodesSortingViewModel(
            SortingColumn sortingColumn,
            bool isSortingByAscendingEnabled)
        {
            _sortingColumn = sortingColumn;
            _isSortingByAscendingEnabled = isSortingByAscendingEnabled;
        }
        
        public void ToggleSortingDirection()
        {
            IsSortingByAscendingEnabled = !IsSortingByAscendingEnabled;
        }
    }
}