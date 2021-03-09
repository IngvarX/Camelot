using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels
{
    public class TabViewModelTests
    {
        private const string CurrentDirectory = "CurrDir";
        private const string CurrentDirectoryName = "CurrDirName";

        private readonly AutoMocker _autoMocker;
        private readonly TabViewModel _tabViewModel;

        public TabViewModelTests()
        {
            _autoMocker = new AutoMocker();
            _autoMocker
                .Setup<IPathService, string>(m => m.TrimPathSeparators(CurrentDirectory))
                .Returns(CurrentDirectory);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileName(CurrentDirectory))
                .Returns(CurrentDirectoryName);
            _autoMocker.Use(CurrentDirectory);

            _tabViewModel = _autoMocker.CreateInstance<TabViewModel>();
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
            Assert.True(_tabViewModel.ActivateCommand.CanExecute(null));
            _tabViewModel.ActivateCommand.Execute(null);

            Assert.True(activationRequested);
        }

        [Fact]
        public void TestNewTabCommand()
        {
            var newTabRequested = false;
            _tabViewModel.NewTabRequested += (sender, args) => newTabRequested = true;
            Assert.True(_tabViewModel.NewTabCommand.CanExecute(null));
            _tabViewModel.NewTabCommand.Execute(null);

            Assert.True(newTabRequested);
        }

        [Fact]
        public void TestNewTabOnSecondPanelCommand()
        {
            var newTabRequested = false;
            _tabViewModel.NewTabOnOtherPanelRequested += (sender, args) => newTabRequested = true;
            Assert.True(_tabViewModel.NewTabOnOtherPanelCommand.CanExecute(null));
            _tabViewModel.NewTabOnOtherPanelCommand.Execute(null);

            Assert.True(newTabRequested);
        }

        [Fact]
        public void TestCloseCommand()
        {
            var closeRequested = false;
            _tabViewModel.CloseRequested += (sender, args) => closeRequested = true;
            Assert.True(_tabViewModel.CloseTabCommand.CanExecute(null));
            _tabViewModel.CloseTabCommand.Execute(null);

            Assert.True(closeRequested);
        }

        [Fact]
        public void TestCloseTabsToTheLeftCommand()
        {
            var closeTabsToTheLeftRequested = false;
            _tabViewModel.ClosingTabsToTheLeftRequested += (sender, args) => closeTabsToTheLeftRequested = true;
            Assert.True(_tabViewModel.CloseTabsToTheLeftCommand.CanExecute(null));
            _tabViewModel.CloseTabsToTheLeftCommand.Execute(null);

            Assert.True(closeTabsToTheLeftRequested);
        }

        [Fact]
        public void TestCloseTabsToTheRightCommand()
        {
            var closeTabsToTheRightRequested = false;
            _tabViewModel.ClosingTabsToTheRightRequested += (sender, args) => closeTabsToTheRightRequested = true;
            Assert.True(_tabViewModel.CloseTabsToTheRightCommand.CanExecute(null));
            _tabViewModel.CloseTabsToTheRightCommand.Execute(null);

            Assert.True(closeTabsToTheRightRequested);
        }

        [Fact]
        public void TestCloseAllTabsButThisCommand()
        {
            var closeAllTabsButThisRequested = false;
            _tabViewModel.ClosingAllTabsButThisRequested += (sender, args) => closeAllTabsButThisRequested = true;
            Assert.True(_tabViewModel.CloseAllTabsButThisCommand.CanExecute(null));
            _tabViewModel.CloseAllTabsButThisCommand.Execute(null);

            Assert.True(closeAllTabsButThisRequested);
        }

        [Fact]
        public void TestSortingViewModelAndDirectory()
        {
            const string directory = "dir";
            _autoMocker
                .Setup<IPathService, string>(m => m.TrimPathSeparators(directory))
                .Returns(directory)
                .Verifiable();
            _autoMocker
                .Setup<IPathService, string>(m => m.GetFileName(directory))
                .Returns(directory)
                .Verifiable();
            _autoMocker.Use(directory);

            var tabViewModel = _autoMocker.CreateInstance<TabViewModel>();

            Assert.Equal(directory, tabViewModel.DirectoryName);
            _autoMocker
                .Verify<IPathService>(m => m.TrimPathSeparators(directory), Times.Once);
            _autoMocker
                .Verify<IPathService>(m => m.GetFileName(directory), Times.Once);
        }
    }
}