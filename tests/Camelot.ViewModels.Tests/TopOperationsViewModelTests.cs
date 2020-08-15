using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class TopOperationsViewModelTests
    {
        private const string Directory = "Directory";

        [Fact]
        public void TestOpenTerminalCommand()
        {
            var terminalServiceMock = new Mock<ITerminalService>();
            terminalServiceMock
                .Setup(m => m.Open(Directory))
                .Verifiable();
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .SetupGet(m => m.SelectedDirectory)
                .Returns(Directory);
            var filesOperationsMediatorMock = new Mock<IFilesOperationsMediator>();

            var viewModel = new TopOperationsViewModel(terminalServiceMock.Object,
                directoryServiceMock.Object, filesOperationsMediatorMock.Object);

            Assert.True(viewModel.OpenTerminalCommand.CanExecute(null));
            viewModel.OpenTerminalCommand.Execute(null);

            terminalServiceMock.Verify(m => m.Open(Directory), Times.Once);
        }

        [Fact]
        public void TestSearchCommand()
        {
            var terminalServiceMock = new Mock<ITerminalService>();
            var directoryServiceMock = new Mock<IDirectoryService>();
            var filesOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            filesOperationsMediatorMock
                .Setup(m => m.ToggleSearchPanelVisibility())
                .Verifiable();

            var viewModel = new TopOperationsViewModel(terminalServiceMock.Object,
                directoryServiceMock.Object, filesOperationsMediatorMock.Object);

            Assert.True(viewModel.SearchCommand.CanExecute(null));
            viewModel.SearchCommand.Execute(null);

            filesOperationsMediatorMock
                .Verify(m => m.ToggleSearchPanelVisibility(), Times.Once);
        }
    }
}