using Camelot.ViewModels.Implementations.MainWindow.FilePanels;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface IFileSystemNodesSortingViewModel
    {
        SortingColumn SortingColumn { get; set; }
        
        bool IsSortingByAscendingEnabled { get; }

        void ToggleSortingDirection();
    }
}