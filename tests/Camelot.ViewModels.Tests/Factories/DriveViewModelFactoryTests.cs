using Camelot.Services.Abstractions;
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
        public void TestCreate()
        {
            var driveModel = new DriveModel();
            var fileSizeFormatterMock = new Mock<IFileSizeFormatter>();
            var pathServiceMock = new Mock<IPathService>();
            var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();

            var factory = new DriveViewModelFactory(fileSizeFormatterMock.Object, pathServiceMock.Object,
                fileOperationsMediatorMock.Object);

            var viewModel = factory.Create(driveModel);

            Assert.NotNull(viewModel);
            Assert.IsType<DriveViewModel>(viewModel);
        }
    }
}