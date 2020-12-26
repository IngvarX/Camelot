using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels
{
    public class FileSystemNodesComparerTests
    {
        private readonly AutoMocker _autoMocker;

        public FileSystemNodesComparerTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(true, SortingMode.Date)]
        [InlineData(false, SortingMode.Name)]
        [InlineData(true, SortingMode.Extension)]
        [InlineData(false, SortingMode.Size)]
        public void TestSortingParentDirectory(bool isAscending, SortingMode sortingColumn)
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