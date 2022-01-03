using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs;

public class OverwriteOptionsDialogViewModelTests
{
    private const string SourceFilePath = "Source";
    private const string DestinationFilePath = "Destination";
    private const string NewFileName = "New";
    private const string ParentDirectory = "Parent";
    private const string ParentDirectoryName = "ParentName";
    private const string NewFilePath = ParentDirectory + NewFileName;

    private readonly AutoMocker _autoMocker;

    public OverwriteOptionsDialogViewModelTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void TestProperties(bool shouldApplyToAll, bool areMultipleFilesAvailable)
    {
        var dialog = Create(areMultipleFilesAvailable);
        dialog.ShouldApplyToAll = shouldApplyToAll;
        dialog.NewFileName = NewFileName;

        Assert.Equal(shouldApplyToAll, dialog.ShouldApplyToAll);
        Assert.Equal(areMultipleFilesAvailable, dialog.AreMultipleFilesAvailable);
        Assert.Equal(ParentDirectoryName, dialog.DestinationDirectoryName);
        Assert.Equal(NewFileName, dialog.NewFileName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TestSkip(bool shouldApplyToAll)
    {
        var isCallbackCalled = false;
        var dialog = Create();
        dialog.ShouldApplyToAll = shouldApplyToAll;
        dialog.CloseRequested += (sender, args) =>
        {
            var result = args.Result;
            Assert.NotNull(result);
            if (result.Options.Mode is OperationContinuationMode.Skip)
            {
                isCallbackCalled = true;
            }

            Assert.Equal(shouldApplyToAll, result.Options.ApplyToAll);
            Assert.Equal(SourceFilePath, result.Options.FilePath);
        };

        Assert.True(dialog.SkipCommand.CanExecute(null));

        dialog.SkipCommand.Execute(null);

        Assert.True(isCallbackCalled);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TestOverwrite(bool shouldApplyToAll)
    {
        var isCallbackCalled = false;
        var dialog = Create();
        dialog.ShouldApplyToAll = shouldApplyToAll;
        dialog.CloseRequested += (sender, args) =>
        {
            var result = args.Result;
            Assert.NotNull(result);
            if (result.Options.Mode is OperationContinuationMode.Overwrite)
            {
                isCallbackCalled = true;
            }

            Assert.Equal(shouldApplyToAll, result.Options.ApplyToAll);
            Assert.Equal(SourceFilePath, result.Options.FilePath);
        };

        Assert.True(dialog.ReplaceCommand.CanExecute(null));
        dialog.ReplaceCommand.Execute(null);

        Assert.True(isCallbackCalled);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TestOverwriteIfOlder(bool shouldApplyToAll)
    {
        var isCallbackCalled = false;
        var dialog = Create();
        dialog.ShouldApplyToAll = shouldApplyToAll;
        dialog.CloseRequested += (sender, args) =>
        {
            var result = args.Result;
            Assert.NotNull(result);
            if (result.Options.Mode is OperationContinuationMode.OverwriteIfOlder)
            {
                isCallbackCalled = true;
            }

            Assert.Equal(shouldApplyToAll, result.Options.ApplyToAll);
            Assert.Equal(SourceFilePath, result.Options.FilePath);
        };

        Assert.True(dialog.ReplaceIfOlderCommand.CanExecute(null));

        dialog.ReplaceIfOlderCommand.Execute(null);

        Assert.True(isCallbackCalled);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TestRename(bool shouldApplyToAll)
    {
        _autoMocker
            .Setup<IFileService, bool>(m => m.CheckIfExists(NewFilePath))
            .Returns(false);

        var isCallbackCalled = false;
        var dialog = Create();
        dialog.ShouldApplyToAll = shouldApplyToAll;
        dialog.NewFileName = NewFileName;
        dialog.CloseRequested += (sender, args) =>
        {
            var result = args.Result;
            Assert.NotNull(result);
            if (result.Options.Mode is OperationContinuationMode.Rename)
            {
                isCallbackCalled = true;
            }

            Assert.Equal(shouldApplyToAll, result.Options.ApplyToAll);
            Assert.Equal(SourceFilePath, result.Options.FilePath);
            Assert.Equal(NewFilePath, result.Options.NewFilePath);
        };

        Assert.True(dialog.RenameCommand.CanExecute(null));

        dialog.RenameCommand.Execute(null);

        Assert.True(isCallbackCalled);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TestRenameNotPossible(bool shouldApplyToAll)
    {
        _autoMocker
            .Setup<IFileService, bool>(m => m.CheckIfExists(NewFilePath))
            .Returns(true);

        var dialog = Create();
        dialog.ShouldApplyToAll = shouldApplyToAll;
        dialog.NewFileName = NewFileName;

        Assert.False(dialog.RenameCommand.CanExecute(null));
    }

    private OverwriteOptionsDialogViewModel Create(bool areMultipleFilesAvailable = true)
    {
        _autoMocker
            .Setup<IFileService, FileModel>(m => m.GetFile(It.IsAny<string>()))
            .Returns<string>(s => new FileModel {FullPath = s, Name = s});
        _autoMocker
            .Setup<IFileSystemNodeViewModelFactory, IFileSystemNodeViewModel>(m => m.Create(It.IsAny<FileModel>()))
            .Returns<FileModel>(fm =>
            {
                var mock = new Mock<IFileSystemNodeViewModel>();
                mock
                    .SetupGet(m => m.FullPath)
                    .Returns(fm.FullPath);

                return mock.Object;
            });
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(DestinationFilePath))
            .Returns(ParentDirectory);
        _autoMocker
            .Setup<IPathService, string>(m => m.Combine(ParentDirectory, NewFileName))
            .Returns(NewFilePath);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetFileName(ParentDirectory))
            .Returns(ParentDirectoryName);

        var dialog = _autoMocker.CreateInstance<OverwriteOptionsDialogViewModel>();

        var parameter = new OverwriteOptionsNavigationParameter(SourceFilePath, DestinationFilePath, areMultipleFilesAvailable);
        dialog.Activate(parameter);

        return dialog;
    }
}