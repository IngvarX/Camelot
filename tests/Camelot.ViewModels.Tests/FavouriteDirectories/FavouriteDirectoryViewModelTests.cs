using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.MainWindow.FavouriteDirectories;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.FavouriteDirectories
{
    public class FavouriteDirectoryViewModelTests
    {
        [Fact]
        public void TestProperties()
        {
            var driveModel = new DirectoryModel
            {
                Name = "Test"
            };

            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();

            var viewModel = new FavouriteDirectoryViewModel(fileOperationsMediatorMock.Object, driveModel);

            Assert.Equal(driveModel.Name, viewModel.DirectoryName);
        }

        [Fact]
        public void TestOpenCommand()
        {
            var directoryModel = new DirectoryModel
            {
                FullPath = "/home/test"
            };
            var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            filePanelViewModelMock
                .SetupSet(m => m.CurrentDirectory = directoryModel.FullPath)
                .Verifiable();
            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            fileOperationsMediatorMock
                .SetupGet(m => m.ActiveFilesPanelViewModel)
                .Returns(filePanelViewModelMock.Object);

            var viewModel = new FavouriteDirectoryViewModel(fileOperationsMediatorMock.Object, directoryModel);

            Assert.True(viewModel.OpenCommand.CanExecute(null));
            viewModel.OpenCommand.Execute(null);

            filePanelViewModelMock
                .VerifySet(m => m.CurrentDirectory = directoryModel.FullPath, Times.Once);
        }
    }
}