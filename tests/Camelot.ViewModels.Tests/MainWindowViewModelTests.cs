using Camelot.ViewModels.Implementations;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
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
            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            fileOperationsMediatorMock
                .Setup(m => m.Register(It.IsAny<IFilesPanelViewModel>(), It.IsAny<IFilesPanelViewModel>()))
                .Verifiable();
            
            var mainWindowViewModel = new MainWindowViewModel(
                fileOperationsMediatorMock.Object,
                operationsViewModelMock.Object,
                filePanelViewModelMock.Object,
                filePanelViewModelMock.Object,
                menuViewModelMock.Object);

            Assert.NotNull(mainWindowViewModel.LeftFilesPanelViewModel);
            Assert.NotNull(mainWindowViewModel.RightFilesPanelViewModel);
            Assert.NotNull(mainWindowViewModel.MenuViewModel);
            Assert.NotNull(mainWindowViewModel.OperationsViewModel);
            
            fileOperationsMediatorMock
                .Verify(m => m.Register(It.IsAny<IFilesPanelViewModel>(), It.IsAny<IFilesPanelViewModel>()));
        }
    }
}