using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class FileSystemNodesSortingViewModel : ViewModelBase, IFileSystemNodesSortingViewModel
    {
        private bool _isSortingByAscendingEnabled;

        [Reactive]
        public SortingMode SortingColumn { get; set; }

        public bool IsSortingByAscendingEnabled
        {
            get => _isSortingByAscendingEnabled;
            private set => this.RaiseAndSetIfChanged(ref _isSortingByAscendingEnabled, value);
        }

        public FileSystemNodesSortingViewModel(
            SortingMode sortingColumn,
            bool isSortingByAscendingEnabled)
        {
            SortingColumn = sortingColumn;
            _isSortingByAscendingEnabled = isSortingByAscendingEnabled;
        }

        public void ToggleSortingDirection() => IsSortingByAscendingEnabled = !IsSortingByAscendingEnabled;
    }
}