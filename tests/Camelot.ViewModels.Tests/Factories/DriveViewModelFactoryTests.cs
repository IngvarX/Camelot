using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories
{
    public class DriveViewModelFactoryTests
    {
        [Fact]
        public void TestCreate()
        {
            var factory = new DriveViewModelFactory();
            var driveModel = new DriveModel
            {
                Name = "Test",
                RootDirectory = "/test"
            };
            var driveViewModel = factory.Create(driveModel);

            Assert.NotNull(driveViewModel);
            Assert.IsType<DriveViewModel>(driveViewModel);
            Assert.Equal(driveModel.RootDirectory, driveViewModel.RootDirectory);
            Assert.Equal(driveModel.Name, ((DriveViewModel) driveViewModel).DriveName);
        }
    }
}