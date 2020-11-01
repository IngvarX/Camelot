using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface IFileSystemNodesSortingViewModel
    {
        SortingMode SortingColumn { get; set; }

        bool IsSortingByAscendingEnabled { get; }

        void ToggleSortingDirection();
    }
}