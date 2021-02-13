using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories
{
    public class DriveViewModelFactoryTests
    {
        [Fact]
        public void TestCreateMounted()
        {
            var driveModel = new DriveModel();
            var fileSizeFormatterMock = new Mock<IFileSizeFormatter>();
            var pathServiceMock = new Mock<IPathService>();
            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            var unmountedDriveServiceMock = new Mock<IUnmountedDriveService>();

            var factory = new DriveViewModelFactory(fileSizeFormatterMock.Object, pathServiceMock.Object,
                fileOperationsMediatorMock.Object, unmountedDriveServiceMock.Object);

            var viewModel = factory.Create(driveModel);

            Assert.NotNull(viewModel);
            Assert.IsType<DriveViewModel>(viewModel);
        }

        [Fact]
        public void TestCreateUnmounted()
        {
            var unmountedDriveModel = new UnmountedDriveModel();
            var fileSizeFormatterMock = new Mock<IFileSizeFormatter>();
            var pathServiceMock = new Mock<IPathService>();
            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
            var unmountedDriveServiceMock = new Mock<IUnmountedDriveService>();

            var factory = new DriveViewModelFactory(fileSizeFormatterMock.Object, pathServiceMock.Object,
                fileOperationsMediatorMock.Object, unmountedDriveServiceMock.Object);

            var viewModel = factory.Create(unmountedDriveModel);

            Assert.NotNull(viewModel);
            Assert.IsType<UnmountedDriveViewModel>(viewModel);
        }
    }
}