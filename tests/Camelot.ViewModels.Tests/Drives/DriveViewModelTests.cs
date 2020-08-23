using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Drives
{
    public class DriveViewModelTests
    {
        [Fact]
        public void TestProperties()
        {
            const string name = "tst";
            var driveModel = new DriveModel
            {
                Name = "Test",
                RootDirectory = "/test",
                TotalSpaceBytes = 42,
                FreeSpaceBytes = 21
            };
            var fileSizeFormatterMock = new Mock<IFileSizeFormatter>();
            fileSizeFormatterMock
                .Setup(m => m.GetSizeAsNumber(It.IsAny<long>()))
                .Returns<long>((bytes) => bytes.ToString());
            fileSizeFormatterMock
                .Setup(m => m.GetFormattedSize(It.IsAny<long>()))
                .Returns<long>((bytes) => bytes + " B");
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetFileName(driveModel.Name))
                .Returns(name);
            var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            fileOperationsMediatorMock
                .SetupGet(m => m.ActiveFilesPanelViewModel)
                .Returns(filePanelViewModelMock.Object);

            var viewModel = new DriveViewModel(fileSizeFormatterMock.Object,
                pathServiceMock.Object, fileOperationsMediatorMock.Object, driveModel);

            Assert.Equal(name, viewModel.DriveName);
            Assert.Equal("21", viewModel.AvailableSizeAsNumber);
            Assert.Equal("21 B", viewModel.AvailableFormattedSize);
            Assert.Equal("42", viewModel.TotalSizeAsNumber);
            Assert.Equal("42 B", viewModel.TotalFormattedSize);
        }

        [Fact]
        public void TestOpenCommand()
        {
            var driveModel = new DriveModel
            {
                Name = "Test",
                RootDirectory = "/test",
                TotalSpaceBytes = 42,
                FreeSpaceBytes = 21
            };
            var fileSizeFormatterMock = new Mock<IFileSizeFormatter>();
            var pathServiceMock = new Mock<IPathService>();
            var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
            filePanelViewModelMock
                .SetupSet(m => m.CurrentDirectory = driveModel.RootDirectory)
                .Verifiable();
            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            fileOperationsMediatorMock
                .SetupGet(m => m.ActiveFilesPanelViewModel)
                .Returns(filePanelViewModelMock.Object);

            var viewModel = new DriveViewModel(fileSizeFormatterMock.Object,
                pathServiceMock.Object, fileOperationsMediatorMock.Object, driveModel);

            Assert.True(viewModel.OpenCommand.CanExecute(null));
            viewModel.OpenCommand.Execute(null);

            filePanelViewModelMock
                .VerifySet(m => m.CurrentDirectory = driveModel.RootDirectory, Times.Once);
        }
    }
}