using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels
{
    public class FileSystemNodesSortingViewModelTests
    {
        [Theory]
        [InlineData(SortingMode.Date, SortingMode.Extension, true)]
        [InlineData(SortingMode.Name, SortingMode.Size, false)]
        public void Test(SortingMode sortingColumn, SortingMode newSortingColumn, bool isSortingByAscendingEnabled)
        {
            var viewModel = new FileSystemNodesSortingViewModel(sortingColumn, isSortingByAscendingEnabled);
            Assert.Equal(sortingColumn, viewModel.SortingColumn);
            Assert.Equal(isSortingByAscendingEnabled, viewModel.IsSortingByAscendingEnabled);

            viewModel.SortingColumn = newSortingColumn;
            Assert.Equal(newSortingColumn, viewModel.SortingColumn);

            viewModel.ToggleSortingDirection();
            Assert.Equal(!isSortingByAscendingEnabled, viewModel.IsSortingByAscendingEnabled);
        }
    }
}