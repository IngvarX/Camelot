using System.Linq;
using Camelot.ViewModels.Implementations;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.Operations;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using Camelot.ViewModels.Interfaces.Menu;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class MainWindowViewModelTests
    {
        private readonly AutoMocker _autoMocker;

        public MainWindowViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestFilePanelsRegistrations()
        {
            var leftFilePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            var tabsListViewModelMock = new Mock<ITabsListViewModel>();
            leftFilePanelViewModelMock
                .Setup(m => m.TabsListViewModel)
                .Returns(tabsListViewModelMock.Object);
            var rightFilePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            var operationsViewModelMock = new Mock<IOperationsViewModel>();
            var menuViewModelMock = new Mock<IMenuViewModel>();
            var operationsStateViewModelMock = new Mock<IOperationsStateViewModel>();
            var topOperationsViewModelMock = new Mock<ITopOperationsViewModel>();
            var drivesListViewModelMock = new Mock<IDrivesListViewModel>();
            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            fileOperationsMediatorMock
                .Setup(m => m.Register(It.IsAny<IFilesPanelViewModel>(), It.IsAny<IFilesPanelViewModel>()))
                .Verifiable();
            fileOperationsMediatorMock
                .Setup(m => m.ActiveFilesPanelViewModel)
                .Returns(leftFilePanelViewModelMock.Object);
            var favouriteDirectoriesListViewModelMock = new Mock<IFavouriteDirectoriesListViewModel>();

            var mainWindowViewModel = new MainWindowViewModel(
                fileOperationsMediatorMock.Object,
                operationsViewModelMock.Object,
                leftFilePanelViewModelMock.Object,
                rightFilePanelViewModelMock.Object,
                menuViewModelMock.Object,
                operationsStateViewModelMock.Object,
                topOperationsViewModelMock.Object,
                drivesListViewModelMock.Object,
                favouriteDirectoriesListViewModelMock.Object);

            Assert.Equal(tabsListViewModelMock.Object, mainWindowViewModel.ActiveTabsListViewModel);
            Assert.Equal(leftFilePanelViewModelMock.Object, mainWindowViewModel.LeftFilesPanelViewModel);
            Assert.Equal(rightFilePanelViewModelMock.Object, mainWindowViewModel.RightFilesPanelViewModel);
            Assert.Equal(menuViewModelMock.Object, mainWindowViewModel.MenuViewModel);
            Assert.Equal(operationsViewModelMock.Object, mainWindowViewModel.OperationsViewModel);
            Assert.Equal(operationsStateViewModelMock.Object, mainWindowViewModel.OperationsStateViewModel);
            Assert.Equal(topOperationsViewModelMock.Object, mainWindowViewModel.TopOperationsViewModel);
            Assert.Equal(drivesListViewModelMock.Object, mainWindowViewModel.DrivesListViewModel);
            Assert.Equal(favouriteDirectoriesListViewModelMock.Object, mainWindowViewModel.FavouriteDirectoriesListViewModel);

            fileOperationsMediatorMock
                .Verify(m => m.Register(It.IsAny<IFilesPanelViewModel>(), It.IsAny<IFilesPanelViewModel>()),
                    Times.Once);
        }

        [Fact]
        public void TestOpenNewTabCommand()
        {
            var tabsListViewModelMock = new Mock<ITabsListViewModel>();
            tabsListViewModelMock
                .Setup(m => m.CreateNewTab(null, true))
                .Verifiable();
            var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            filePanelViewModelMock
                .SetupGet(m => m.TabsListViewModel)
                .Returns(tabsListViewModelMock.Object);
            _autoMocker
                .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
                .Returns(filePanelViewModelMock.Object);

            var mainWindowViewModel = _autoMocker.CreateInstance<MainWindowViewModel>();

            Assert.True(mainWindowViewModel.CreateNewTabCommand.CanExecute(null));
            mainWindowViewModel.CreateNewTabCommand.Execute(null);

            tabsListViewModelMock
                .Verify(m => m.CreateNewTab(null, true), Times.Once);
        }

        [Fact]
        public void TestCloseCurrentTabCommand()
        {
            var tabsListViewModelMock = new Mock<ITabsListViewModel>();
            tabsListViewModelMock
                .Setup(m => m.CloseActiveTab())
                .Verifiable();
            var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            filePanelViewModelMock
                .SetupGet(m => m.TabsListViewModel)
                .Returns(tabsListViewModelMock.Object);
            _autoMocker
                .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
                .Returns(filePanelViewModelMock.Object);

            var mainWindowViewModel = _autoMocker.CreateInstance<MainWindowViewModel>();

            Assert.True(mainWindowViewModel.CloseCurrentTabCommand.CanExecute(null));
            mainWindowViewModel.CloseCurrentTabCommand.Execute(null);

            tabsListViewModelMock
                .Verify(m => m.CloseActiveTab(), Times.Once);
        }

        [Fact]
        public void TestSearchCommand()
        {
            _autoMocker
                .Setup<IFilesOperationsMediator>(m => m.ToggleSearchPanelVisibility())
                .Verifiable();

            var mainWindowViewModel = _autoMocker.CreateInstance<MainWindowViewModel>();

            Assert.True(mainWindowViewModel.SearchCommand.CanExecute(null));
            mainWindowViewModel.SearchCommand.Execute(null);

            _autoMocker
                .Verify<IFilesOperationsMediator>(m => m.ToggleSearchPanelVisibility(), Times.Once);
        }

        [Fact]
        public void TestSwitchPanelCommand()
        {
            var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            _autoMocker
                .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.InactiveFilesPanelViewModel)
                .Returns(filePanelViewModelMock.Object);

            var mainWindowViewModel = _autoMocker.CreateInstance<MainWindowViewModel>();

            Assert.True(mainWindowViewModel.SwitchPanelCommand.CanExecute(null));
            mainWindowViewModel.SwitchPanelCommand.Execute(null);

            filePanelViewModelMock.Verify(m => m.Activate(), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(100)]
        public void TestGoToTabCommand(int tabIndex)
        {
            var tabListViewModelMock = new Mock<ITabsListViewModel>();
            tabListViewModelMock
                .Setup(m => m.SelectTab(tabIndex))
                .Verifiable();
            var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            filePanelViewModelMock
                .Setup(m => m.TabsListViewModel)
                .Returns(tabListViewModelMock.Object);
            _autoMocker
                .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
                .Returns(filePanelViewModelMock.Object);

            var mainWindowViewModel = _autoMocker.CreateInstance<MainWindowViewModel>();

            Assert.True(mainWindowViewModel.GoToTabCommand.CanExecute(null));
            mainWindowViewModel.GoToTabCommand.Execute(tabIndex);

            tabListViewModelMock
                .Verify(m => m.SelectTab(tabIndex), Times.Once);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(3, 2)]
        [InlineData(5, 4)]
        [InlineData(100, 99)]
        public void TestGoToLastTabCommand(int tabsCount, int tabIndex)
        {
            var tabs = Enumerable
                .Repeat(new Mock<ITabViewModel>().Object, tabsCount)
                .ToArray();
            var tabListViewModelMock = new Mock<ITabsListViewModel>();
            tabListViewModelMock
                .Setup(m => m.SelectTab(tabIndex))
                .Verifiable();
            tabListViewModelMock
                .Setup(m => m.Tabs)
                .Returns(tabs);
            var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            filePanelViewModelMock
                .Setup(m => m.TabsListViewModel)
                .Returns(tabListViewModelMock.Object);
            _autoMocker
                .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
                .Returns(filePanelViewModelMock.Object);

            var mainWindowViewModel = _autoMocker.CreateInstance<MainWindowViewModel>();

            Assert.True(mainWindowViewModel.GoToLastTabCommand.CanExecute(null));
            mainWindowViewModel.GoToLastTabCommand.Execute(null);

            tabListViewModelMock
                .Verify(m => m.SelectTab(tabIndex), Times.Once);
        }
    }
}