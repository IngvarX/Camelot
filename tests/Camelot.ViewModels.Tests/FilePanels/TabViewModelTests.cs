using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels
{
    public class TabViewModelTests
    {
        private const string CurrentDirectory = "CurrDir";
        private const string CurrentDirectoryName = "CurrDirName";

        private readonly TabViewModel _tabViewModel;

        public TabViewModelTests()
        {
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.TrimPathSeparators(CurrentDirectory))
                .Returns(CurrentDirectory);
            pathServiceMock
                .Setup(m => m.GetFileName(CurrentDirectory))
                .Returns(CurrentDirectoryName);
            var sortingViewModelMock = new Mock<IFileSystemNodesSortingViewModel>();

            _tabViewModel = new TabViewModel(pathServiceMock.Object, sortingViewModelMock.Object, CurrentDirectory);
        }

        [Fact]
        public void TestProperties()
        {
            Assert.Equal(CurrentDirectory, _tabViewModel.CurrentDirectory);
            Assert.Equal(CurrentDirectoryName, _tabViewModel.DirectoryName);
            Assert.False(_tabViewModel.IsActive);
            Assert.False(_tabViewModel.IsGloballyActive);

            _tabViewModel.IsActive = _tabViewModel.IsGloballyActive = true;
            Assert.True(_tabViewModel.IsActive);
            Assert.True(_tabViewModel.IsGloballyActive);
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