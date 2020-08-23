using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories
{
    public class DriveViewModelFactoryTests
    {
        [Fact]
        public void TestCreate()
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
            var factory = new DriveViewModelFactory(fileSizeFormatterMock.Object, pathServiceMock.Object);

            var viewModel = factory.Create(driveModel);

            Assert.NotNull(viewModel);
            Assert.IsType<DriveViewModel>(viewModel);

            var driveViewModel = (DriveViewModel) viewModel;
            Assert.Equal(driveModel.RootDirectory, driveViewModel.RootDirectory);
            Assert.Equal(name, driveViewModel.DriveName);
            Assert.Equal("21", driveViewModel.AvailableSizeAsNumber);
            Assert.Equal("21 B", driveViewModel.AvailableFormattedSize);
            Assert.Equal("42", driveViewModel.TotalSizeAsNumber);
            Assert.Equal("42 B", driveViewModel.TotalFormattedSize);
        }
    }
}