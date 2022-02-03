using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs;

public class CreateDirectoryDialogViewModelTests
{
    private const string DirectoryPath = "Directory";
    private const string DirectoryName = "Name";
    private const string NewDirectoryPath = "Directory/Name";

    private readonly AutoMocker _autoMocker;

    public CreateDirectoryDialogViewModelTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void TestDirectoryWithWhiteSpaceCreation(string directoryName)
    {
        var dialog = _autoMocker.CreateInstance<CreateDirectoryDialogViewModel>();
        dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

        Assert.False(dialog.CreateCommand.CanExecute(null));

        dialog.DirectoryName = directoryName;

        Assert.False(dialog.CreateCommand.CanExecute(null));
    }

    [Fact]
    public void TestDirectoryWithExistingNodeNameCreation()
    {
        _autoMocker
            .Setup<INodeService, bool>(m => m.CheckIfExists(NewDirectoryPath))
            .Returns(true);
        _autoMocker
            .Setup<IPathService, string>(m => m.Combine(DirectoryPath, DirectoryName))
            .Returns(NewDirectoryPath);

        var dialog = _autoMocker.CreateInstance<CreateDirectoryDialogViewModel>();
        dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

        dialog.DirectoryName = DirectoryName;

        Assert.False(dialog.CreateCommand.CanExecute(null));
    }

    [Fact]
    public void TestDirectoryCreation()
    {
        var dialog = _autoMocker.CreateInstance<CreateDirectoryDialogViewModel>();
        dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

        var isCallbackCalled = false;
        dialog.CloseRequested += (sender, args) =>
        {
            if (args.Result.DirectoryName == DirectoryName)
            {
                isCallbackCalled = true;
            }
        };

        dialog.DirectoryName = DirectoryName;

        Assert.Equal(DirectoryName, dialog.DirectoryName);
        Assert.True(dialog.CreateCommand.CanExecute(null));

        dialog.CreateCommand.Execute(null);

        Assert.True(isCallbackCalled);
    }
}