using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories;

public class DriveViewModelFactoryTests
{
    private readonly AutoMocker _autoMocker;

    public DriveViewModelFactoryTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestCreateMounted()
    {
        var driveModel = new DriveModel();
        var factory = _autoMocker.CreateInstance<DriveViewModelFactory>();

        var viewModel = factory.Create(driveModel);

        Assert.NotNull(viewModel);
        Assert.IsType<DriveViewModel>(viewModel);
    }

    [Fact]
    public void TestCreateUnmounted()
    {
        var unmountedDriveModel = new UnmountedDriveModel();
        var factory = _autoMocker.CreateInstance<DriveViewModelFactory>();

        var viewModel = factory.Create(unmountedDriveModel);

        Assert.NotNull(viewModel);
        Assert.IsType<UnmountedDriveViewModel>(viewModel);
    }
}