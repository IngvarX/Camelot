using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class FileSystemNodesSortingViewModelTests
    {
        [Theory]
        [InlineData(SortingColumn.Date, SortingColumn.Extension, true)]
        [InlineData(SortingColumn.Name, SortingColumn.Size, false)]
        public void Test(SortingColumn sortingColumn, SortingColumn newSortingColumn, bool isSortingByAscendingEnabled)
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