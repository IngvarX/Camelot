using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class FileSystemNodeViewModelBaseTests
    {
        private const string FullPath = "FullPath";
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