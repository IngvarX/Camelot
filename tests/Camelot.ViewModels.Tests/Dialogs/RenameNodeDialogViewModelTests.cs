using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs;

public class RenameNodeDialogViewModelTests
{
    private const string NodePath = "NodePath";
    private const string NodeDirectory = "NodeDir";
    private const string NodeName = "NodeName";
    private const string NewNodePath = "Node/Name";
    private const string NewNodeName = "NewName";

    private readonly AutoMocker _autoMocker;

    public RenameNodeDialogViewModelTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestProperties()
    {
        _autoMocker
            .Setup<IPathService, string>(m => m.GetFileName(NodePath))
            .Returns(NodeName);

        var dialog = _autoMocker.CreateInstance<RenameNodeDialogViewModel>();
        dialog.Activate(new RenameNodeNavigationParameter(NodePath));

        Assert.Equal(NodeName, dialog.NodeName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void TestNodeWithWhiteSpaceName(string nodeName)
    {
        var dialog = _autoMocker.CreateInstance<RenameNodeDialogViewModel>();
        dialog.Activate(new RenameNodeNavigationParameter(NodePath));

        Assert.False(dialog.RenameCommand.CanExecute(null));

        dialog.NodeName = nodeName;

        Assert.False(dialog.RenameCommand.CanExecute(null));
    }

    [Fact]
    public void TestNodeWithExistingNodeName()
    {
        _autoMocker
            .Setup<INodeService, bool>(m => m.CheckIfExists(NewNodePath))
            .Returns(true);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(NodePath))
            .Returns(NodeDirectory);
        _autoMocker
            .Setup<IPathService, string>(m => m.Combine(NodeDirectory, NewNodeName))
            .Returns(NewNodePath);

        var dialog = _autoMocker.CreateInstance<RenameNodeDialogViewModel>();
        dialog.Activate(new RenameNodeNavigationParameter(NodePath));

        dialog.NodeName = NewNodeName;

        Assert.False(dialog.RenameCommand.CanExecute(null));
    }

    [Fact]
    public void TestRename()
    {
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(NodePath))
            .Returns(NodeDirectory);
        _autoMocker
            .Setup<IPathService, string>(m => m.Combine(NodeDirectory, NewNodeName))
            .Returns(NewNodePath);

        var dialog = _autoMocker.CreateInstance<RenameNodeDialogViewModel>();
        dialog.Activate(new RenameNodeNavigationParameter(NodePath));

        var isCallbackCalled = false;
        dialog.CloseRequested += (sender, args) =>
        {
            if (args.Result.NodeName == NewNodePath)
            {
                isCallbackCalled = true;
            }
        };

        dialog.NodeName = NewNodeName;

        Assert.Equal(NewNodeName, dialog.NodeName);
        Assert.True(dialog.RenameCommand.CanExecute(null));

        dialog.RenameCommand.Execute(null);

        Assert.True(isCallbackCalled);
    }
}