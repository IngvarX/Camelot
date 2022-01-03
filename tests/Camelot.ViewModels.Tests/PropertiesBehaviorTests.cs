using System.Threading.Tasks;
using Camelot.ViewModels.Implementations.Behaviors;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Properties;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests;

public class PropertiesBehaviorTests
{
    private const string Path = "path";

    [Fact]
    public async Task TestDirectoryPropertiesBehavior()
    {
        FileSystemNodeNavigationParameter parameter = null;
        var dialogServiceMock = new Mock<IDialogService>();
        dialogServiceMock
            .Setup(m => m.ShowDialogAsync(
                nameof(DirectoryInformationDialogViewModel), It.IsAny<FileSystemNodeNavigationParameter>()))
            .Callback<string, FileSystemNodeNavigationParameter>((_, p) => parameter = p)
            .Returns(Task.CompletedTask);

        var behavior = new DirectoryPropertiesBehavior(dialogServiceMock.Object);
        await behavior.ShowPropertiesAsync(Path);

        Assert.NotNull(parameter);
        Assert.Equal(Path, parameter.NodePath);
    }

    [Fact]
    public async Task TestFilePropertiesBehavior()
    {
        FileSystemNodeNavigationParameter parameter = null;
        var dialogServiceMock = new Mock<IDialogService>();
        dialogServiceMock
            .Setup(m => m.ShowDialogAsync(
                nameof(FileInformationDialogViewModel), It.IsAny<FileSystemNodeNavigationParameter>()))
            .Callback<string, FileSystemNodeNavigationParameter>((_, p) => parameter = p)
            .Returns(Task.CompletedTask);

        var behavior = new FilePropertiesBehavior(dialogServiceMock.Object);
        await behavior.ShowPropertiesAsync(Path);

        Assert.NotNull(parameter);
        Assert.Equal(Path, parameter.NodePath);
    }
}