using Camelot.ViewModels.Implementations;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
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
                .Setup(m => m.CreateNewTab())
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
                .Verify(m => m.CreateNewTab(), Times.Once);
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
    }
}