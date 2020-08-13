using Camelot.ViewModels.Implementations;
using Camelot.ViewModels.Interfaces.MainWindow;
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
            var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            var operationsViewModelMock = new Mock<IOperationsViewModel>();
            var menuViewModelMock = new Mock<IMenuViewModel>();
            var operationsStateViewModel = new Mock<IOperationsStateViewModel>();
            var topOperationsViewModelMock = new Mock<ITopOperationsViewModel>();
            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            fileOperationsMediatorMock
                .Setup(m => m.Register(It.IsAny<IFilesPanelViewModel>(), It.IsAny<IFilesPanelViewModel>()))
                .Verifiable();

            var mainWindowViewModel = new MainWindowViewModel(
                fileOperationsMediatorMock.Object,
                operationsViewModelMock.Object,
                filePanelViewModelMock.Object,
                filePanelViewModelMock.Object,
                menuViewModelMock.Object,
                operationsStateViewModel.Object,
                topOperationsViewModelMock.Object);

            Assert.NotNull(mainWindowViewModel.LeftFilesPanelViewModel);
            Assert.NotNull(mainWindowViewModel.RightFilesPanelViewModel);
            Assert.NotNull(mainWindowViewModel.MenuViewModel);
            Assert.NotNull(mainWindowViewModel.OperationsViewModel);
            Assert.NotNull(mainWindowViewModel.OperationsStateViewModel);
            Assert.NotNull(mainWindowViewModel.TopOperationsViewModel);

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

            var mainWindowViewModel = new MainWindowViewModel(
                fileOperationsMediatorMock.Object,
                operationsViewModelMock.Object,
                filePanelViewModelMock.Object,
                filePanelViewModelMock.Object,
                menuViewModelMock.Object,
                operationsStateViewModel.Object,
                topOperationsViewModelMock.Object);

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

            var mainWindowViewModel = new MainWindowViewModel(
                fileOperationsMediatorMock.Object,
                operationsViewModelMock.Object,
                filePanelViewModelMock.Object,
                filePanelViewModelMock.Object,
                menuViewModelMock.Object,
                operationsStateViewModel.Object,
                topOperationsViewModelMock.Object);

            mainWindowViewModel.CloseCurrentTabCommand.Execute(null);

            tabsListViewModelMock
                .Verify(m => m.CloseActiveTab(), Times.Once);
        }
    }
}