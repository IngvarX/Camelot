using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Drives;

public class DriveViewModelTests
{
    private readonly AutoMocker _autoMocker;

    public DriveViewModelTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData(true, Platform.Linux)]
    [InlineData(true, Platform.MacOs)]
    [InlineData(false, Platform.Windows)]
    [InlineData(false, Platform.Unknown)]
    public void TestProperties(bool isEjectAvailable, Platform platform)
    {
        const string name = "tst";
        var driveModel = new DriveModel
        {
            Name = "Test",
            RootDirectory = "/test",
            TotalSpaceBytes = 42,
            FreeSpaceBytes = 21
        };
        _autoMocker.Use(driveModel);
        _autoMocker
            .Setup<IFileSizeFormatter, string>(m => m.GetSizeAsNumber(It.IsAny<long>()))
            .Returns<long>((bytes) => bytes.ToString());
        _autoMocker
            .Setup<IFileSizeFormatter, string>(m => m.GetFormattedSize(It.IsAny<long>()))
            .Returns<long>((bytes) => bytes + " B");
        _autoMocker
            .Setup<IPathService, string>(m => m.GetFileName(driveModel.Name))
            .Returns(name);
        var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
            .Returns(filePanelViewModelMock.Object);
        _autoMocker
            .Setup<IPlatformService, Platform>(m => m.GetPlatform())
            .Returns(platform);

        var viewModel = _autoMocker.CreateInstance<DriveViewModel>();

        Assert.Equal(name, viewModel.DriveName);
        Assert.Equal("21", viewModel.AvailableSizeAsNumber);
        Assert.Equal("21 B", viewModel.AvailableFormattedSize);
        Assert.Equal("42", viewModel.TotalSizeAsNumber);
        Assert.Equal("42 B", viewModel.TotalFormattedSize);
        Assert.Equal(isEjectAvailable, viewModel.IsEjectAvailable);
        Assert.Equal(driveModel.Name, viewModel.Name);
        Assert.Equal(driveModel.TotalSpaceBytes, viewModel.TotalSpaceBytes);
        Assert.Equal(driveModel.FreeSpaceBytes, viewModel.FreeSpaceBytes);
        Assert.Equal(driveModel.RootDirectory, viewModel.RootDirectory);
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
        _autoMocker.Use(driveModel);
        var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
        _autoMocker
            .GetMock<IFilesPanelViewModel>()
            .SetupSet(m => m.CurrentDirectory = driveModel.RootDirectory)
            .Verifiable();
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
            .Returns(filePanelViewModelMock.Object);

        var viewModel = _autoMocker.CreateInstance<DriveViewModel>();

        Assert.True(viewModel.OpenCommand.CanExecute(null));
        viewModel.OpenCommand.Execute(null);

        filePanelViewModelMock
            .VerifySet(m => m.CurrentDirectory = driveModel.RootDirectory, Times.Once);
    }

    [Fact]
    public void TestUnmountCommand()
    {
        var driveModel = new DriveModel
        {
            RootDirectory = "/test"
        };
        _autoMocker.Use(driveModel);

        var viewModel = _autoMocker.CreateInstance<DriveViewModel>();

        Assert.True(viewModel.UnmountCommand.CanExecute(null));
        viewModel.UnmountCommand.Execute(null);

        _autoMocker
            .Verify<IMountedDriveService>(m => m.Unmount(driveModel.RootDirectory),
                Times.Once);
    }

    [Fact]
    public void TestEjectCommand()
    {
        var driveModel = new DriveModel
        {
            RootDirectory = "/test"
        };
        _autoMocker.Use(driveModel);

        var viewModel = _autoMocker.CreateInstance<DriveViewModel>();

        Assert.True(viewModel.EjectCommand.CanExecute(null));
        viewModel.EjectCommand.Execute(null);

        _autoMocker
            .Verify<IMountedDriveService>(m => m.EjectAsync(driveModel.RootDirectory),
                Times.Once);
    }
}