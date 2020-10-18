using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface IFileSystemNodesSortingViewModel
    {
        SortingColumn SortingColumn { get; set; }
        
        bool IsSortingByAscendingEnabled { get; }

        void ToggleSortingDirection();
    }
}