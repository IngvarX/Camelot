using Camelot.ViewModels.Implementations;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using Camelot.ViewModels.Interfaces.Menu;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class MainWindowViewModelTests
    {
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

            var mainWindowViewModel = new MainWindowViewModel(
                fileOperationsMediatorMock.Object,
                operationsViewModelMock.Object,
                leftFilePanelViewModelMock.Object,
                rightFilePanelViewModelMock.Object,
                menuViewModelMock.Object,
                operationsStateViewModelMock.Object,
                topOperationsViewModelMock.Object,
                drivesListViewModelMock.Object);

            Assert.Equal(leftFilePanelViewModelMock.Object, mainWindowViewModel.LeftFilesPanelViewModel);
            Assert.Equal(rightFilePanelViewModelMock.Object, mainWindowViewModel.RightFilesPanelViewModel);
            Assert.Equal(menuViewModelMock.Object, mainWindowViewModel.MenuViewModel);
            Assert.Equal(operationsViewModelMock.Object, mainWindowViewModel.OperationsViewModel);
            Assert.Equal(operationsStateViewModelMock.Object, mainWindowViewModel.OperationsStateViewModel);
            Assert.Equal(topOperationsViewModelMock.Object, mainWindowViewModel.TopOperationsViewModel);
            Assert.Equal(drivesListViewModelMock.Object, mainWindowViewModel.DrivesListViewModel);

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
            var operationsViewModelMock = new Mock<IOperationsViewModel>();
            var menuViewModelMock = new Mock<IMenuViewModel>();
            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            fileOperationsMediatorMock
                .SetupGet(m => m.ActiveFilesPanelViewModel)
                .Returns(filePanelViewModelMock.Object);
            var operationsStateViewModel = new Mock<IOperationsStateViewModel>();
            var topOperationsViewModelMock = new Mock<ITopOperationsViewModel>();
            var drivesListViewModelMock = new Mock<IDrivesListViewModel>();

            var mainWindowViewModel = new MainWindowViewModel(
                fileOperationsMediatorMock.Object,
                operationsViewModelMock.Object,
                filePanelViewModelMock.Object,
                filePanelViewModelMock.Object,
                menuViewModelMock.Object,
                operationsStateViewModel.Object,
                topOperationsViewModelMock.Object,
                drivesListViewModelMock.Object);

            Assert.True(mainWindowViewModel.CreateNewTabCommand.CanExecute(null));
            mainWindowViewModel.CreateNewTabCommand.Execute(null);

            tabsListViewModelMock
                .Verify(m => m.CreateNewTab(), Times.Once);        }

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
            var operationsViewModelMock = new Mock<IOperationsViewModel>();
            var menuViewModelMock = new Mock<IMenuViewModel>();
            var operationsStateViewModel = new Mock<IOperationsStateViewModel>();
            var topOperationsViewModelMock = new Mock<ITopOperationsViewModel>();
            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            fileOperationsMediatorMock
                .SetupGet(m => m.ActiveFilesPanelViewModel)
                .Returns(filePanelViewModelMock.Object);
            var drivesListViewModelMock = new Mock<IDrivesListViewModel>();

            var mainWindowViewModel = new MainWindowViewModel(
                fileOperationsMediatorMock.Object,
                operationsViewModelMock.Object,
                filePanelViewModelMock.Object,
                filePanelViewModelMock.Object,
                menuViewModelMock.Object,
                operationsStateViewModel.Object,
                topOperationsViewModelMock.Object,
                drivesListViewModelMock.Object);

            Assert.True(mainWindowViewModel.CloseCurrentTabCommand.CanExecute(null));
            mainWindowViewModel.CloseCurrentTabCommand.Execute(null);

            tabsListViewModelMock
                .Verify(m => m.CloseActiveTab(), Times.Once);
        }

        [Fact]
        public void TestSearchCommand()
        {
            var tabsListViewModelMock = new Mock<ITabsListViewModel>();
            var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            var operationsViewModelMock = new Mock<IOperationsViewModel>();
            var menuViewModelMock = new Mock<IMenuViewModel>();
            var operationsStateViewModel = new Mock<IOperationsStateViewModel>();
            var topOperationsViewModelMock = new Mock<ITopOperationsViewModel>();
            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            fileOperationsMediatorMock
                .Setup(m => m.ToggleSearchPanelVisibility())
                .Verifiable();
            var drivesListViewModelMock = new Mock<IDrivesListViewModel>();

            var mainWindowViewModel = new MainWindowViewModel(
                fileOperationsMediatorMock.Object,
                operationsViewModelMock.Object,
                filePanelViewModelMock.Object,
                filePanelViewModelMock.Object,
                menuViewModelMock.Object,
                operationsStateViewModel.Object,
                topOperationsViewModelMock.Object,
                drivesListViewModelMock.Object);

            Assert.True(mainWindowViewModel.SearchCommand.CanExecute(null));
            mainWindowViewModel.SearchCommand.Execute(null);

            fileOperationsMediatorMock
                .Verify(m => m.ToggleSearchPanelVisibility(), Times.Once);
        }
    }
}