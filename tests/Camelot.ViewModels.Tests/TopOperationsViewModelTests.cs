using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class TopOperationsViewModelTests
    {
        private const string Directory = "Directory";

        private readonly AutoMocker _autoMocker;

        public TopOperationsViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestOpenTerminalCommand()
        {
            _autoMocker
                .Setup<ITerminalService>(m => m.Open(Directory))
                .Verifiable();
            _autoMocker
                .Setup<IDirectoryService, string>(m => m.SelectedDirectory)
                .Returns(Directory);

            var viewModel = _autoMocker.CreateInstance<TopOperationsViewModel>();

            Assert.True(viewModel.OpenTerminalCommand.CanExecute(null));
            viewModel.OpenTerminalCommand.Execute(null);

            _autoMocker.Verify<ITerminalService>(m => m.Open(Directory), Times.Once);
        }

        [Fact]
        public void TestSearchCommand()
        {
            _autoMocker
                .Setup<IFilesOperationsMediator>(m => m.ToggleSearchPanelVisibility())
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<TopOperationsViewModel>();

            Assert.True(viewModel.SearchCommand.CanExecute(null));
            viewModel.SearchCommand.Execute(null);

            _autoMocker
                .Verify<IFilesOperationsMediator>(m => m.ToggleSearchPanelVisibility(), Times.Once);
        }
    }
}