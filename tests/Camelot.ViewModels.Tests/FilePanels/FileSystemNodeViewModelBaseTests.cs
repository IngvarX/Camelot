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
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using ReactiveUI;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels
{
    public class FileSystemNodeViewModelBaseTests
    {
        private const string FullPath = "FullPath";
        private const string Extension = "Extension";
        private const string FullName = "FullName";
        private const string DestinationDirectory = "DestDir";

        private readonly AutoMocker _autoMocker;

        public FileSystemNodeViewModelBaseTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void TestIsArchive(bool isArchive, bool expected)
        {
            _autoMocker
                .Setup<IArchiveService, bool>(m => m.CheckIfNodeIsArchive(FullPath))
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
                .Setup<IClipboardOperationsService>(m => m.CopyFilesAsync(
                    It.Is<IReadOnlyList<string>>(f => f.Single() == FullPath)))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
            viewModel.FullPath = FullPath;

            Assert.True(viewModel.CopyToClipboardCommand.CanExecute(null));

            viewModel.CopyToClipboardCommand.Execute(null);

            _autoMocker
                .Verify<IClipboardOperationsService>(m => m.CopyFilesAsync(
                        It.Is<IReadOnlyList<string>>(f => f.Single() == FullPath)),
                    Times.Once);
        }

        [Fact]
        public void TestCopyCommand()
        {
            _autoMocker
                .Setup<IOperationsService>(m => m.CopyAsync(
                    It.Is<IReadOnlyList<string>>(n => n.Single() == FullPath), DestinationDirectory))
                .Verifiable();
            _autoMocker
                .Setup<IFilesOperationsMediator, string>(m => m.OutputDirectory)
                .Returns(DestinationDirectory);

            var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
            viewModel.FullPath = FullPath;

            Assert.True(viewModel.CopyCommand.CanExecute(null));

            viewModel.CopyCommand.Execute(null);

            _autoMocker
                .Verify<IOperationsService>(m => m.CopyAsync(
                    It.Is<IReadOnlyList<string>>(n => n.Single() == FullPath), DestinationDirectory),
            Times.Once());
        }

        [Fact]
        public void TestMoveCommand()
        {
            _autoMocker
                .Setup<IOperationsService>(m => m.MoveAsync(
                    It.Is<IReadOnlyList<string>>(n => n.Single() == FullPath), DestinationDirectory))
                .Verifiable();
            _autoMocker
                .Setup<IFilesOperationsMediator, string>(m => m.OutputDirectory)
                .Returns(DestinationDirectory);

            var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
            viewModel.FullPath = FullPath;

            Assert.True(viewModel.MoveCommand.CanExecute(null));

            viewModel.MoveCommand.Execute(null);

            _autoMocker
                .Verify<IOperationsService>(m => m.MoveAsync(
                        It.Is<IReadOnlyList<string>>(n => n.Single() == FullPath), DestinationDirectory),
                    Times.Once());
        }

        [Theory]
        [InlineData("", true, false, 0)]
        [InlineData(null, true, false, 0)]
        [InlineData("new", true, false, 1)]
        [InlineData("new", false, true, 1)]
        public void TestRenameCommand(string newName, bool isEditing, bool isRenameSuccessful, int renameTimesCalled)
        {
            _autoMocker
                .Setup<IOperationsService, bool>(m => m.Rename(FullPath, newName))
                .Returns(isRenameSuccessful);

            var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
            viewModel.FullPath = FullPath;
            viewModel.FullName = newName;
            viewModel.IsEditing = true;

            Assert.True(viewModel.RenameCommand.CanExecute(null));

            viewModel.RenameCommand.Execute(null);

            Assert.Equal(isEditing, viewModel.IsEditing);

            _autoMocker
                .Verify<IOperationsService, bool>(m => m.Rename(FullPath, newName),
                    Times.Exactly(renameTimesCalled));
        }

        [Theory]
        [InlineData(null, 0)]
        [InlineData("new", 1)]
        public void TestRenameInDialogCommand(string newName, int renameTimesCalled)
        {
            var result = newName is null ? null : new RenameNodeDialogResult(newName);
            _autoMocker
                .Setup<IDialogService, Task<RenameNodeDialogResult>>(m => m.ShowDialogAsync<RenameNodeDialogResult, RenameNodeNavigationParameter>(
                    nameof(RenameNodeDialogViewModel), It.Is<RenameNodeNavigationParameter>(
                        p => p.NodePath == FullPath)))
                .Returns(Task.FromResult(result));
            _autoMocker
                .Setup<IOperationsService, bool>(m => m.Rename(FullPath, newName))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
            viewModel.FullPath = FullPath;

            Assert.True(viewModel.RenameInDialogCommand.CanExecute(null));

            viewModel.RenameInDialogCommand.Execute(null);

            _autoMocker
                .Verify<IOperationsService, bool>(m => m.Rename(FullPath, newName),
                    Times.Exactly(renameTimesCalled));
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        [InlineData(null, 0)]
        public void TestDeleteCommand(bool? shouldRemove, int removingCallsCount)
        {
            var result = shouldRemove.HasValue ? new RemoveNodesConfirmationDialogResult(shouldRemove.Value) : null;
            _autoMocker
                .Setup<IDialogService, Task<RemoveNodesConfirmationDialogResult>>(m => m.ShowDialogAsync<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>(
                        nameof(RemoveNodesConfirmationDialogViewModel), It.Is<NodesRemovingNavigationParameter>(
                            p => p.IsRemovingToTrash && p.Files.Single() == FullPath)))
                .Returns(Task.FromResult(result));
            _autoMocker
                .Setup<ITrashCanService>(m => m.MoveToTrashAsync(It.Is<IReadOnlyList<string>>(
                    f => f.Single() == FullPath), It.IsAny<CancellationToken>()))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
            viewModel.FullPath = FullPath;

            Assert.True(viewModel.DeleteCommand.CanExecute(null));

            viewModel.DeleteCommand.Execute(null);

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
        public void TestExtractCommand(bool isArchive, ExtractCommandType command,
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

            var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
            viewModel.FullPath = FullPath;

            Assert.True(viewModel.ExtractCommand.CanExecute(command));

            viewModel.ExtractCommand.Execute(command);

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
        public void TestExtractCommandThrows()
        {
            _autoMocker
                .Setup<IArchiveService, bool>(m => m.CheckIfNodeIsArchive(FullPath))
                .Returns(true);

            var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
            viewModel.FullPath = FullPath;

            const ExtractCommandType command = (ExtractCommandType) 42;

            void ExecuteCommand() => viewModel.ExtractCommand.Execute(command);

            Assert.Throws<UnhandledErrorException>(ExecuteCommand);
        }

        [Theory]
        [InlineData(true, "path", ArchiveType.Bz2, 1)]
        [InlineData(true, "path2", ArchiveType.Zip, 1)]
        [InlineData(false, "path3", ArchiveType.TarGz, 0)]
        [InlineData(true, "path4", ArchiveType.Tar, 1)]
        public void TestPackCommand(bool returnResult, string archivePath, ArchiveType archiveType,
            int timesCalled)
        {
            _autoMocker
                .Setup<IArchiveService>(m => m.PackAsync(
                    It.Is<IReadOnlyList<string>>(f => f.Single() == FullPath),
                    archivePath, archiveType))
                .Verifiable();
            var result = returnResult ? new CreateArchiveDialogResult(archivePath, archiveType) : null;
            _autoMocker
                .Setup<IDialogService, Task<CreateArchiveDialogResult>>(m => m.ShowDialogAsync<CreateArchiveDialogResult, CreateArchiveNavigationParameter>(
                        nameof(CreateArchiveDialogViewModel), It.Is<CreateArchiveNavigationParameter>(
                            p => p.IsPackingSingleFile && p.DefaultArchivePath == FullPath)))
                .Returns(Task.FromResult(result));

            var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
            viewModel.FullPath = FullPath;

            Assert.True(viewModel.PackCommand.CanExecute(null));

            viewModel.PackCommand.Execute(null);

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
        public void TestOpenWithCommand(bool returnResult, bool isDefaultApplication,
            int openWithTimesCalled, int saveSelectedAppCalled)
        {
            var app = new ApplicationModel
            {
                ExecutePath = "path",
                Arguments = "args"
            };
            var result = returnResult ? new OpenWithDialogResult(Extension, app, isDefaultApplication) : null;
            _autoMocker
                .Setup<IDialogService, Task<OpenWithDialogResult>>(m => m.ShowDialogAsync<OpenWithDialogResult, OpenWithNavigationParameter>(
                    nameof(OpenWithDialogViewModel), It.Is<OpenWithNavigationParameter>(
                        p => p.FileExtension == Extension && p.Application == app)))
                .Returns(Task.FromResult(result));
            _autoMocker
                .Setup<IPathService, string>(m => m.GetExtension(FullName))
                .Returns(Extension);
            _autoMocker
                .Setup<IOpenWithApplicationService, ApplicationModel>(m => m.GetSelectedApplication(Extension))
                .Returns(app);
            _autoMocker
                .Setup<IFileSystemNodeOpeningBehavior>(m => m.OpenWith(
                    app.ExecutePath, app.Arguments, FullPath))
                .Verifiable();
            _autoMocker
                .Setup<IOpenWithApplicationService>(m => m.SaveSelectedApplication(
                    Extension, app))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<NodeViewModel>();
            viewModel.FullPath = FullPath;
            viewModel.FullName = FullName;

            Assert.True(viewModel.OpenWithCommand.CanExecute(null));

            viewModel.OpenWithCommand.Execute(null);

            _autoMocker
                .Verify<IFileSystemNodeOpeningBehavior>(m => m.OpenWith(
                        app.ExecutePath, app.Arguments, FullPath),
                    Times.Exactly(openWithTimesCalled));
            _autoMocker
                .Verify<IOpenWithApplicationService>(m => m.SaveSelectedApplication(
                        Extension, app),
                    Times.Exactly(saveSelectedAppCalled));
        }

        private class NodeViewModel : FileSystemNodeViewModelBase
        {
            public NodeViewModel(
                IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
                IOperationsService operationsService,
                IClipboardOperationsService clipboardOperationsService,
                IFilesOperationsMediator filesOperationsMediator,
                IFileSystemNodePropertiesBehavior fileSystemNodePropertiesBehavior,
                IDialogService dialogService,
                ITrashCanService trashCanService,
                IArchiveService archiveService,
                ISystemDialogService systemDialogService,
                IOpenWithApplicationService openWithApplicationService,
                IPathService pathService)
                : base(
                    fileSystemNodeOpeningBehavior,
                    operationsService,
                    clipboardOperationsService,
                    filesOperationsMediator,
                    fileSystemNodePropertiesBehavior,
                    dialogService,
                    trashCanService,
                    archiveService,
                    systemDialogService,
                    openWithApplicationService,
                    pathService)
            {

            }
        }
    }
}