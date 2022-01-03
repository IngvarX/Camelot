using Camelot.Services.Abstractions.Behaviors;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels;

public class FileSystemNodeViewModelBaseTests
{
    private const string FullPath = "FullPath";
    private const string FullName = "FullName";

    private readonly AutoMocker _autoMocker;

    public FileSystemNodeViewModelBaseTests()
    {
        _autoMocker = new AutoMocker();
        _autoMocker.Use(false);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void TestShouldShowOpenSubmenu(bool shouldShow, bool expected)
    {
        _autoMocker.Use(shouldShow);

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();

        Assert.Equal(expected, viewModel.ShouldShowOpenSubmenu);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void TestIsArchive(bool isArchive, bool expected)
    {
        _autoMocker
            .Setup<IFileSystemNodeFacade, bool>(m => m.CheckIfNodeIsArchive(FullPath))
            .Returns(isArchive);

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
        viewModel.FullPath = FullPath;

        Assert.Equal(expected, viewModel.IsArchive);
    }

    [Fact]
    public void TestOpenCommand()
    {
        _autoMocker
            .Setup<IFileSystemNodeOpeningBehavior>(m => m.Open(FullPath))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
        viewModel.FullPath = FullPath;

        Assert.True(viewModel.OpenCommand.CanExecute(null));

        viewModel.OpenCommand.Execute(null);

        _autoMocker
            .Verify<IFileSystemNodeOpeningBehavior>(m => m.Open(FullPath),
                Times.Once);
    }

    [Fact]
    public void TestShowPropertiesCommand()
    {
        _autoMocker
            .Setup<IFileSystemNodePropertiesBehavior>(m => m.ShowPropertiesAsync(FullPath))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
        viewModel.FullPath = FullPath;

        Assert.True(viewModel.ShowPropertiesCommand.CanExecute(null));

        viewModel.ShowPropertiesCommand.Execute(null);

        _autoMocker
            .Verify<IFileSystemNodePropertiesBehavior>(m => m.ShowPropertiesAsync(FullPath),
                Times.Once);
    }

    [Fact]
    public void TestCopyToClipboardCommand()
    {
        _autoMocker
            .Setup<IFileSystemNodeFacade>(m => m.CopyToClipboardAsync(FullPath))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
        viewModel.FullPath = FullPath;

        Assert.True(viewModel.CopyToClipboardCommand.CanExecute(null));

        viewModel.CopyToClipboardCommand.Execute(null);

        _autoMocker
            .Verify<IFileSystemNodeFacade>(m => m.CopyToClipboardAsync(FullPath),
                Times.Once);
    }

    [Fact]
    public void TestCopyCommand()
    {
        _autoMocker
            .Setup<IFileSystemNodeFacade>(m => m.CopyAsync(FullPath))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
        viewModel.FullPath = FullPath;

        Assert.True(viewModel.CopyCommand.CanExecute(null));

        viewModel.CopyCommand.Execute(null);

        _autoMocker
            .Verify<IFileSystemNodeFacade>(m => m.CopyAsync(FullPath),
                Times.Once);
    }

    [Fact]
    public void TestMoveCommand()
    {
        _autoMocker
            .Setup<IFileSystemNodeFacade>(m => m.MoveAsync(FullPath))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
        viewModel.FullPath = FullPath;

        Assert.True(viewModel.MoveCommand.CanExecute(null));

        viewModel.MoveCommand.Execute(null);

        _autoMocker
            .Verify<IFileSystemNodeFacade>(m => m.MoveAsync(FullPath),
                Times.Once);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void TestRenameCommand(bool isEditing, bool isRenameSuccessful)
    {
        _autoMocker
            .Setup<IFileSystemNodeFacade, bool>(m => m.Rename(FullName, FullPath))
            .Returns(isRenameSuccessful);

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
        viewModel.FullPath = FullPath;
        viewModel.FullName = FullName;
        viewModel.IsEditing = true;

        Assert.True(viewModel.RenameCommand.CanExecute(null));

        viewModel.RenameCommand.Execute(null);

        Assert.Equal(isEditing, viewModel.IsEditing);
    }

    [Fact]
    public void TestRenameInDialogCommand()
    {
        _autoMocker
            .Setup<IFileSystemNodeFacade>(m => m.RenameInDialogAsync(FullPath))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
        viewModel.FullPath = FullPath;

        Assert.True(viewModel.RenameInDialogCommand.CanExecute(null));

        viewModel.RenameInDialogCommand.Execute(null);

        _autoMocker
            .Verify<IFileSystemNodeFacade>(m => m.RenameInDialogAsync(FullPath),
                Times.Once);
    }

    [Fact]
    public void TestDeleteCommand()
    {
        _autoMocker
            .Setup<IFileSystemNodeFacade>(m => m.DeleteAsync(FullPath))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
        viewModel.FullPath = FullPath;

        Assert.True(viewModel.DeleteCommand.CanExecute(null));

        viewModel.DeleteCommand.Execute(null);

        _autoMocker
            .Verify<IFileSystemNodeFacade>(m => m.DeleteAsync(FullPath),
                Times.Once);
    }

    [Theory]
    [InlineData(ExtractCommandType.CurrentDirectory)]
    [InlineData(ExtractCommandType.NewDirectory)]
    [InlineData(ExtractCommandType.SelectDirectory)]
    public void TestExtractCommand(ExtractCommandType command)
    {
        _autoMocker
            .Setup<IFileSystemNodeFacade>(m => m.ExtractAsync(command, FullPath))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
        viewModel.FullPath = FullPath;

        Assert.True(viewModel.ExtractCommand.CanExecute(command));

        viewModel.ExtractCommand.Execute(command);

        _autoMocker
            .Verify<IFileSystemNodeFacade>(m => m.ExtractAsync(command, FullPath),
                Times.Once);
    }

    [Fact]
    public void TestPackCommand()
    {
        _autoMocker
            .Setup<IFileSystemNodeFacade>(m => m.PackAsync(FullPath))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
        viewModel.FullPath = FullPath;

        Assert.True(viewModel.PackCommand.CanExecute(null));

        viewModel.PackCommand.Execute(null);

        _autoMocker
            .Verify<IFileSystemNodeFacade>(m => m.PackAsync(FullPath),
                Times.Once);
    }

    [Fact]
    public void TestOpenWithCommand()
    {
        _autoMocker
            .Setup<IFileSystemNodeFacade>(m => m.OpenWithAsync(
                _autoMocker.GetMock<IFileSystemNodeOpeningBehavior>().Object, FullPath))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
        viewModel.FullPath = FullPath;
        viewModel.FullName = FullName;

        Assert.True(viewModel.OpenWithCommand.CanExecute(null));

        viewModel.OpenWithCommand.Execute(null);

        _autoMocker
            .Verify<IFileSystemNodeFacade>(m => m.OpenWithAsync(
                    _autoMocker.GetMock<IFileSystemNodeOpeningBehavior>().Object, FullPath),
                Times.Once);
    }

    private class NodeViewModel : FileSystemNodeViewModelBase
    {
        public NodeViewModel(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
            IFileSystemNodePropertiesBehavior fileSystemNodePropertiesBehavior,
            IFileSystemNodeFacade fileSystemNodeFacade,
            bool shouldShowOpenSubmenu)
            : base(
                fileSystemNodeOpeningBehavior,
                fileSystemNodePropertiesBehavior,
                fileSystemNodeFacade,
                shouldShowOpenSubmenu)
        {

        }
    }
}