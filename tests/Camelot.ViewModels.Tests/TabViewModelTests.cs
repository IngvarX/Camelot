using System.Globalization;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class TabViewModelTests
    {
        private readonly TabViewModel _tabViewModel;

        public TabViewModelTests()
        {
            var pathServiceMock = new Mock<IPathService>();
            var sortingViewModelMock = new Mock<IFileSystemNodesSortingViewModel>();

            _tabViewModel = new TabViewModel(pathServiceMock.Object, sortingViewModelMock.Object, string.Empty);
        }

        [Fact]
        public void TestActivateCommand()
        {
            var activationRequested = false;
            _tabViewModel.ActivationRequested += (sender, args) => activationRequested = true;
            _tabViewModel.ActivateCommand.Execute(null);

            Assert.True(activationRequested);
        }

        [Fact]
        public void TestNewTabCommand()
        {
            var newTabRequested = false;
            _tabViewModel.NewTabRequested += (sender, args) => newTabRequested = true;
            _tabViewModel.NewTabCommand.Execute(null);

            Assert.True(newTabRequested);
        }

        [Fact]
        public void TestCloseCommand()
        {
            var closeRequested = false;
            _tabViewModel.CloseRequested += (sender, args) => closeRequested = true;
            _tabViewModel.CloseTabCommand.Execute(null);

            Assert.True(closeRequested);
        }

        [Fact]
        public void TestCloseTabsToTheLeftCommand()
        {
            var closeTabsToTheLeftRequested = false;
            _tabViewModel.ClosingTabsToTheLeftRequested += (sender, args) => closeTabsToTheLeftRequested = true;
            _tabViewModel.CloseTabsToTheLeftCommand.Execute(null);

            Assert.True(closeTabsToTheLeftRequested);
        }

        [Fact]
        public void TestCloseTabsToTheRightCommand()
        {
            var closeTabsToTheRightRequested = false;
            _tabViewModel.ClosingTabsToTheRightRequested += (sender, args) => closeTabsToTheRightRequested = true;
            _tabViewModel.CloseTabsToTheRightCommand.Execute(null);

            Assert.True(closeTabsToTheRightRequested);
        }

        [Fact]
        public void TestCloseAllTabsButThisCommand()
        {
            var closeAllTabsButThisRequested = false;
            _tabViewModel.ClosingAllTabsButThisRequested += (sender, args) => closeAllTabsButThisRequested = true;
            _tabViewModel.CloseAllTabsButThisCommand.Execute(null);

            Assert.True(closeAllTabsButThisRequested);
        }

        [Fact]
        public void TestSortingViewModelAndDirectory()
        {
            const string directory = "dir";
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.TrimPathSeparators(directory))
                .Returns(directory)
                .Verifiable();
            pathServiceMock
                .Setup(m => m.GetFileName(directory))
                .Returns(directory)
                .Verifiable();
            var sortingViewModelMock = new Mock<IFileSystemNodesSortingViewModel>();

            var tabViewModel = new TabViewModel(pathServiceMock.Object, sortingViewModelMock.Object, directory);
            Assert.Equal(sortingViewModelMock.Object, tabViewModel.SortingViewModel);

            Assert.Equal(directory, tabViewModel.DirectoryName);
            pathServiceMock.Verify(m => m.TrimPathSeparators(directory), Times.Once);
            pathServiceMock.Verify(m => m.GetFileName(directory), Times.Once);
        }
    }
}