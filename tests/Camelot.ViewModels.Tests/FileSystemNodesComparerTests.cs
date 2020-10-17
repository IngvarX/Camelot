using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class FileSystemNodesComparerTests
    {
        private readonly AutoMocker _autoMocker;

        public FileSystemNodesComparerTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(true, SortingColumn.Date)]
        [InlineData(false, SortingColumn.Name)]
        [InlineData(true, SortingColumn.Extension)]
        [InlineData(false, SortingColumn.Size)]
        public void TestSortingParentDirectory(bool isAscending, SortingColumn sortingColumn)
        {
            var parentDirectoryViewModel =  _autoMocker.CreateInstance<DirectoryViewModel>();
            parentDirectoryViewModel.IsParentDirectory = true;
            var directoryViewModel =  _autoMocker.CreateInstance<DirectoryViewModel>();

            var comparer = new FileSystemNodesComparer(isAscending, sortingColumn);

            var result = comparer.Compare(parentDirectoryViewModel, directoryViewModel);
            Assert.True(result < 0);

            result = comparer.Compare(directoryViewModel, parentDirectoryViewModel);
            Assert.True(result > 0);
        }
    }
}