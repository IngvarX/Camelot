using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories
{
    public class FileSystemNodeViewModelComparerFactoryTests
    {
        [Theory]
        [InlineData(SortingColumn.Date, true)]
        [InlineData(SortingColumn.Size, false)]
        public void TestCreate(SortingColumn sortingColumn, bool isSortingByAscendingEnabled)
        {
            var viewModelMock = new Mock<IFileSystemNodesSortingViewModel>();
            viewModelMock
                .SetupGet(m => m.SortingColumn)
                .Returns(sortingColumn);
            viewModelMock
                .SetupGet(m => m.IsSortingByAscendingEnabled)
                .Returns(isSortingByAscendingEnabled);

            var factory = new FileSystemNodeViewModelComparerFactory();
            var comparer = factory.Create(viewModelMock.Object);

            Assert.NotNull(comparer);
            Assert.IsType<FileSystemNodesComparer>(comparer);
        }
    }
}