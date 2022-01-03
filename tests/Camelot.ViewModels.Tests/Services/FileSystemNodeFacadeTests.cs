using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Camelot.ViewModels.Services.Implementations;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Services;

public class FileSystemNodeFacadeTests
{
    private const string FullPath = "FullPath";
    private const string Extension = "Extension";
    private const string FullName = "FullName";
    private const string DestinationDirectory = "DestDir";

    private readonly AutoMocker _autoMocker;

    public FileSystemNodeFacadeTests()
    {
        _autoMocker = new AutoMocker();
        _autoMocker.Use(false);
    }


    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void TestIsArchive(bool isArchive, bool expected)
    {
        _autoMocker
            .Setup<IArchiveService, bool>(m => m.CheckIfNodeIsArchive(FullPath))
            .Returns(isArchive);

        var facade = _autoMocker.CreateInstance<FileSystemNodeFacade>();

        Assert.Equal(expected, facade.CheckIfNodeIsArchive(FullPath));
    }

    [Fact]
    public async Task TestCopyToClipboard()
    {
        _autoMocker
            .Setup<IClipboardOperationsService>(m => m.CopyFilesAsync(
                It.Is<IReadOnlyList<string>>(f => f.Single() == FullPath)))
            .Verifiable();

        var facade = _autoMocker.CreateInstance<FileSystemNodeFacade>();

        await facade.CopyToClipboardAsync(FullPath);

        _autoMocker
            .Verify<IClipboardOperationsService>(m => m.CopyFilesAsync(
                    It.Is<IReadOnlyList<string>>(f => f.Single() == FullPath)),
                Times.Once);
    }

    [Fact]
    public async Task TestCopy()
    {
        _autoMocker
            .Setup<IOperationsService>(m => m.CopyAsync(
                It.Is<IReadOnlyList<string>>(n => n.Single() == FullPath), DestinationDirectory))
            .Verifiable();
        _autoMocker
            .Setup<IFilesOperationsMediator, string>(m => m.OutputDirectory)
            .Returns(DestinationDirectory);

        var facade = _autoMocker.CreateInstance<FileSystemNodeFacade>();

        await facade.CopyAsync(FullPath);

        _autoMocker
            .Verify<IOperationsService>(m => m.CopyAsync(
                    It.Is<IReadOnlyList<string>>(n => n.Single() == FullPath), DestinationDirectory),
                Times.Once());
    }

    [Fact]
    public async Task TestMoveCommand()
    {
        _autoMocker
            .Setup<IOperationsService>(m => m.MoveAsync(
                It.Is<IReadOnlyList<string>>(n => n.Single() == FullPath), DestinationDirectory))
            .Verifiable();
        _autoMocker
            .Setup<IFilesOperationsMediator, string>(m => m.OutputDirectory)
            .Returns(DestinationDirectory);

        var facade = _autoMocker.CreateInstance<FileSystemNodeFacade>();

        await facade.MoveAsync(FullPath);

        _autoMocker
            .Verify<IOperationsService>(m => m.MoveAsync(
                    It.Is<IReadOnlyList<string>>(n => n.Single() == FullPath), DestinationDirectory),
                Times.Once());
    }

    [Theory]
    [InlineData("", false, false, 0)]
    [InlineData(null, false, false, 0)]
    [InlineData("new", false, false, 1)]
    [InlineData("new", true, true, 1)]
    public void TestRename(string newName, bool expected, bool isRenameSuccessful, int renameTimesCalled)
    {
        _autoMocker
            .Setup<IOperationsService, bool>(m => m.Rename(FullPath, newName))
            .Returns(isRenameSuccessful);

        var facade = _autoMocker.CreateInstance<FileSystemNodeFacade>();

        var result = facade.Rename(newName, FullPath);
        Assert.Equal(expected, result);

        _autoMocker
            .Verify<IOperationsService, bool>(m => m.Rename(FullPath, newName),
                Times.Exactly(renameTimesCalled));
    }


    [Theory]
    [InlineData(null, 0)]
    [InlineData("new", 1)]
    public async Task TestRenameInDialog(string newName, int renameTimesCalled)
    {
        var result = newName is null ? null : new RenameNodeDialogResult(newName);
        _autoMocker
            .Setup<IDialogService, Task<RenameNodeDialogResult>>(m =>
                m.ShowDialogAsync<RenameNodeDialogResult, RenameNodeNavigationParameter>(
                    nameof(RenameNodeDialogViewModel), It.Is<RenameNodeNavigationParameter>(
                        p => p.NodePath == FullPath)))
            .Returns(Task.FromResult(result));
        _autoMocker
            .Setup<IOperationsService, bool>(m => m.Rename(FullPath, newName))
            .Verifiable();

        var facade = _autoMocker.CreateInstance<FileSystemNodeFacade>();

        await facade.RenameInDialogAsync(FullPath);

        _autoMocker
            .Verify<IOperationsService, bool>(m => m.Rename(FullPath, newName),
                Times.Exactly(renameTimesCalled));
    }

    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    [InlineData(null, 0)]
    public async Task TestDeleteCommand(bool? shouldRemove, int removingCallsCount)
    {
        var result = shouldRemove.HasValue ? new RemoveNodesConfirmationDialogResult(shouldRemove.Value) : null;
        _autoMocker
            .Setup<IDialogService, Task<RemoveNodesConfirmationDialogResult>>(m =>
                m.ShowDialogAsync<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>(
                    nameof(RemoveNodesConfirmationDialogViewModel), It.Is<NodesRemovingNavigationParameter>(
                        p => p.IsRemovingToTrash && p.Files.Single() == FullPath)))
            .Returns(Task.FromResult(result));
        _autoMocker
            .Setup<ITrashCanService>(m => m.MoveToTrashAsync(It.Is<IReadOnlyList<string>>(
                f => f.Single() == FullPath), It.IsAny<CancellationToken>()))
            .Verifiable();

        var facade = _autoMocker.CreateInstance<FileSystemNodeFacade>();

        await facade.DeleteAsync(FullPath);

        _autoMocker
            .Verify<ITrashCanService>(m => m.MoveToTrashAsync(It.Is<IReadOnlyList<string>>(
                    f => f.Single() == FullPath), It.IsAny<CancellationToken>()),
                Times.Exactly(removingCallsCount));
    }

    [Theory]
    [InlineData(false, ExtractCommandType.CurrentDirectory, 0, 0, 0, "dir")]
    [InlineData(false, ExtractCommandType.NewDirectory, 0, 0, 0, "dir")]
    [InlineData(false, ExtractCommandType.SelectDirectory, 0, 0, 0, "dir")]
    [InlineData(true, ExtractCommandType.CurrentDirectory, 1, 0, 0, "dir")]
    [InlineData(true, ExtractCommandType.NewDirectory, 0, 1, 0, "dir")]
    [InlineData(true, ExtractCommandType.SelectDirectory, 0, 0, 1, "dir")]
    [InlineData(true, ExtractCommandType.SelectDirectory, 0, 0, 0, null)]
    [InlineData(true, ExtractCommandType.SelectDirectory, 0, 0, 0, "")]
    [InlineData(true, ExtractCommandType.SelectDirectory, 0, 0, 0, "   \t")]
    public async Task TestExtractCommand(bool isArchive, ExtractCommandType command,
        int extractCallsCount, int extractToNewDirCount, int extractWithDirCallsCount, string directory)
    {
        _autoMocker
            .Setup<IArchiveService, bool>(m => m.CheckIfNodeIsArchive(FullPath))
            .Returns(isArchive);
        _autoMocker
            .Setup<IArchiveService>(m => m.ExtractAsync(FullPath, null))
            .Verifiable();
        _autoMocker
            .Setup<IArchiveService>(m => m.ExtractAsync(FullPath, directory))
            .Verifiable();
        _autoMocker
            .Setup<IArchiveService>(m => m.ExtractToNewDirectoryAsync(FullPath))
            .Verifiable();
        _autoMocker
            .Setup<ISystemDialogService, Task<string>>(m => m.GetDirectoryAsync(null))
            .Returns(Task.FromResult(directory));

        var facade = _autoMocker.CreateInstance<FileSystemNodeFacade>();

        await facade.ExtractAsync(command, FullPath);

        _autoMocker
            .Verify<IArchiveService>(m => m.ExtractAsync(FullPath, null),
                Times.Exactly(extractCallsCount));
        _autoMocker
            .Verify<IArchiveService>(m => m.ExtractAsync(FullPath, directory),
                Times.Exactly(extractWithDirCallsCount));
        _autoMocker
            .Verify<IArchiveService>(m => m.ExtractToNewDirectoryAsync(FullPath),
                Times.Exactly(extractToNewDirCount));
    }

    [Fact]
    public async Task TestExtractCommandThrows()
    {
        _autoMocker
            .Setup<IArchiveService, bool>(m => m.CheckIfNodeIsArchive(FullPath))
            .Returns(true);

        var facade = _autoMocker.CreateInstance<FileSystemNodeFacade>();

        const ExtractCommandType command = (ExtractCommandType) 42;

        Task ExtractAsync() => facade.ExtractAsync(command, FullPath);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(ExtractAsync);
    }

    [Theory]
    [InlineData(true, "path", ArchiveType.Bz2, 1)]
    [InlineData(true, "path2", ArchiveType.Zip, 1)]
    [InlineData(false, "path3", ArchiveType.TarGz, 0)]
    [InlineData(true, "path4", ArchiveType.Tar, 1)]
    public async Task TestPack(bool returnResult, string archivePath, ArchiveType archiveType,
        int timesCalled)
    {
        _autoMocker
            .Setup<IArchiveService>(m => m.PackAsync(
                It.Is<IReadOnlyList<string>>(f => f.Single() == FullPath),
                archivePath, archiveType))
            .Verifiable();
        var result = returnResult ? new CreateArchiveDialogResult(archivePath, archiveType) : null;
        _autoMocker
            .Setup<IDialogService, Task<CreateArchiveDialogResult>>(m =>
                m.ShowDialogAsync<CreateArchiveDialogResult, CreateArchiveNavigationParameter>(
                    nameof(CreateArchiveDialogViewModel), It.Is<CreateArchiveNavigationParameter>(
                        p => p.IsPackingSingleFile && p.DefaultArchivePath == FullPath)))
            .Returns(Task.FromResult(result));

        var facade = _autoMocker.CreateInstance<FileSystemNodeFacade>();

        await facade.PackAsync(FullPath);

        _autoMocker
            .Verify<IArchiveService>(m => m.PackAsync(
                    It.Is<IReadOnlyList<string>>(f => f.Single() == FullPath),
                    archivePath, archiveType),
                Times.Exactly(timesCalled));
    }

    [Theory]
    [InlineData(false, true, 0, 0)]
    [InlineData(true, true, 1, 1)]
    [InlineData(true, false, 1, 0)]
    public async Task TestOpenWith(bool returnResult, bool isDefaultApplication,
        int openWithTimesCalled, int saveSelectedAppCalled)
    {
        var app = new ApplicationModel
        {
            ExecutePath = "path",
            Arguments = "args"
        };
        var result = returnResult ? new OpenWithDialogResult(Extension, app, isDefaultApplication) : null;
        _autoMocker
            .Setup<IDialogService, Task<OpenWithDialogResult>>(m =>
                m.ShowDialogAsync<OpenWithDialogResult, OpenWithNavigationParameter>(
                    nameof(OpenWithDialogViewModel), It.Is<OpenWithNavigationParameter>(
                        p => p.FileExtension == Extension && p.Application == app)))
            .ReturnsAsync(result);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetExtension(FullPath))
            .Returns(Extension);
        _autoMocker
            .Setup<IOpenWithApplicationService, ApplicationModel>(m => m.GetSelectedApplication(Extension))
            .Returns(app);
        var behavior = new Mock<IFileSystemNodeOpeningBehavior>();
        behavior
            .Setup(m => m.OpenWith(
                app.ExecutePath, app.Arguments, FullPath))
            .Verifiable();
        _autoMocker
            .Setup<IOpenWithApplicationService>(m => m.SaveSelectedApplication(
                Extension, app))
            .Verifiable();

        var facade = _autoMocker.CreateInstance<FileSystemNodeFacade>();

        await facade.OpenWithAsync(behavior.Object, FullPath);

        behavior
            .Verify(m => m.OpenWith(
                    app.ExecutePath, app.Arguments, FullPath),
                Times.Exactly(openWithTimesCalled));
        _autoMocker
            .Verify<IOpenWithApplicationService>(m => m.SaveSelectedApplication(
                    Extension, app),
                Times.Exactly(saveSelectedAppCalled));
    }
}