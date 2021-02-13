using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Drives
{
    public class UnmountedDriveViewModelTests
    {
        [Fact]
        public void TestProperties()
        {
            var unmountedDriveServiceMock = new Mock<IUnmountedDriveService>();
            var unmountedDriveModel = new UnmountedDriveModel
            {
                Name = "sda1"
            };
            var viewModel = new UnmountedDriveViewModel(unmountedDriveServiceMock.Object, unmountedDriveModel);

            Assert.Equal(unmountedDriveModel.Name, viewModel.DriveName);
        }

        [Fact]
        public void TestMount()
        {
            var unmountedDriveModel = new UnmountedDriveModel
            {
                FullName = "/dev/sda1/"
            };

            var unmountedDriveServiceMock = new Mock<IUnmountedDriveService>();
            unmountedDriveServiceMock
                .Setup(m => m.Mount(unmountedDriveModel.FullName))
                .Verifiable();

            var viewModel = new UnmountedDriveViewModel(unmountedDriveServiceMock.Object, unmountedDriveModel);

            Assert.True(viewModel.MountCommand.CanExecute(null));
            viewModel.MountCommand.Execute(null);

            unmountedDriveServiceMock
                .Verify(m => m.Mount(unmountedDriveModel.FullName), Times.Once);
        }
    }
}