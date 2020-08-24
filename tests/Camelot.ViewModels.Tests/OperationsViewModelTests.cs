using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class OperationsViewModelTests
    {
        private const string Directory = "Directory";
        private const int DelayMs = 1000;

        private readonly AutoMocker _autoMocker;
        private readonly string[] _files;

        public OperationsViewModelTests()
        {
            _autoMocker = new AutoMocker();
            _files = new[] {"File1", "File2"};
        }

        [Fact]
        public void TestOpen()
        {
            var filesPanelViewModelMock = new Mock<IFilesPanelViewModel>();
            filesPanelViewModelMock
                .Setup(m => m.OpenLastSelectedFile())
                .Verifiable();
            _autoMocker
                .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
                .Returns(filesPanelViewModelMock.Object);

            var viewModel = _autoMocker.CreateInstance<OperationsViewModel>();

            Assert.True(viewModel.OpenCommand.CanExecute(null));
            viewModel.OpenCommand.Execute(null);

            filesPanelViewModelMock
                .Verify(m => m.OpenLastSelectedFile(), Times.Once);
        }

        [Fact]
        public void TestOpenInDefaultEditor()
        {
            _autoMocker
                .Setup<IOperationsService>(m => m.OpenFiles(_files))
                .Verifiable();
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(_files);

            var viewModel = _autoMocker.CreateInstance<OperationsViewModel>();

            Assert.True(viewModel.OpenInDefaultEditorCommand.CanExecute(null));
            viewModel.OpenInDefaultEditorCommand.Execute(null);

            _autoMocker
                .Verify<IOperationsService>(m => m.OpenFiles(_files), Times.Once);
        }

        [Fact]
        public async Task TestCopy()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            _autoMocker
                .Setup<IOperationsService>(m => m.CopyAsync(_files, Directory))
                .Callback(() => taskCompletionSource.SetResult(true))
                .Verifiable();
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(_files);
            _autoMocker
                .Setup<IFilesOperationsMediator, string>(m => m.OutputDirectory)
                .Returns(Directory);

            var viewModel = _autoMocker.CreateInstance<OperationsViewModel>();

            Assert.True(viewModel.CopyCommand.CanExecute(null));
            viewModel.CopyCommand.Execute(null);

            await Task.WhenAny(Task.Delay(DelayMs), taskCompletionSource.Task);

            _autoMocker
                .Verify<IOperationsService>(m => m.CopyAsync(_files, Directory), Times.Once);
        }

        [Fact]
        public async Task TestMove()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            _autoMocker
                .Setup<IOperationsService>(m => m.MoveAsync(_files, Directory))
                .Callback(() => taskCompletionSource.SetResult(true))
                .Verifiable();
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(_files);
            _autoMocker
                .Setup<IFilesOperationsMediator, string>(m => m.OutputDirectory)
                .Returns(Directory);

            var viewModel = _autoMocker.CreateInstance<OperationsViewModel>();

            Assert.True(viewModel.MoveCommand.CanExecute(null));
            viewModel.MoveCommand.Execute(null);

            await Task.WhenAny(Task.Delay(DelayMs), taskCompletionSource.Task);

            _autoMocker
                .Verify<IOperationsService>(m => m.MoveAsync(_files, Directory), Times.Once);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("Test", true)]
        public async Task TestCreateNewDirectory(string directoryName, bool isCalled)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            _autoMocker
                .Setup<IOperationsService>(m => m.CreateDirectory(Directory, directoryName))
                .Callback(() => taskCompletionSource.SetResult(true))
                .Verifiable();
            _autoMocker
                .Setup<IDirectoryService, string>(m => m.SelectedDirectory)
                .Returns(Directory);
            _autoMocker
                .Setup<IDialogService, Task<CreateDirectoryDialogResult>>(m =>
                    m.ShowDialogAsync<CreateDirectoryDialogResult, CreateNodeNavigationParameter>(
                        nameof(CreateDirectoryDialogViewModel), It.Is<CreateNodeNavigationParameter>(p =>
                            p.DirectoryPath == Directory)))
                .Returns(Task.FromResult(new CreateDirectoryDialogResult(directoryName)));

            var viewModel = _autoMocker.CreateInstance<OperationsViewModel>();

            Assert.True(viewModel.CreateNewDirectoryCommand.CanExecute(null));
            viewModel.CreateNewDirectoryCommand.Execute(null);

            await Task.WhenAny(Task.Delay(DelayMs), taskCompletionSource.Task);

            _autoMocker
                .Verify<IOperationsService>(m => m.CreateDirectory(Directory, directoryName),
                    isCalled ? Times.Once() : Times.Never());
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("Test", true)]
        public async Task TestCreateNewFile(string fileName, bool isCalled)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            _autoMocker
                .Setup<IOperationsService>(m => m.CreateFile(Directory, fileName))
                .Callback(() => taskCompletionSource.SetResult(true))
                .Verifiable();
            _autoMocker
                .Setup<IDirectoryService, string>(m => m.SelectedDirectory)
                .Returns(Directory);
            _autoMocker
                .Setup<IDialogService, Task<CreateFileDialogResult>>(m =>
                    m.ShowDialogAsync<CreateFileDialogResult, CreateNodeNavigationParameter>(
                        nameof(CreateFileDialogViewModel), It.Is<CreateNodeNavigationParameter>(p =>
                            p.DirectoryPath == Directory)))
                .Returns(Task.FromResult(new CreateFileDialogResult(fileName)));

            var viewModel = _autoMocker.CreateInstance<OperationsViewModel>();

            Assert.True(viewModel.CreateNewFileCommand.CanExecute(null));
            viewModel.CreateNewFileCommand.Execute(null);

            await Task.WhenAny(Task.Delay(DelayMs), taskCompletionSource.Task);

            _autoMocker
                .Verify<IOperationsService>(m => m.CreateFile(Directory, fileName),
                    isCalled ? Times.Once() : Times.Never());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestRemove(bool isConfirmed)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            _autoMocker
                .Setup<IOperationsService>(m => m.RemoveAsync(_files))
                .Callback(() => taskCompletionSource.SetResult(true))
                .Verifiable();
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(_files);
            _autoMocker
                .Setup<IDialogService, Task<RemoveNodesConfirmationDialogResult>>(m =>
                    m.ShowDialogAsync<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>(
                        nameof(RemoveNodesConfirmationDialogViewModel), It.Is<NodesRemovingNavigationParameter>(p =>
                            !p.IsRemovingToTrash && p.Files.Count == _files.Length && p.Files[0] == _files[0] && p.Files[1] == _files[1])))
                .Returns(Task.FromResult(new RemoveNodesConfirmationDialogResult(isConfirmed)));

            var viewModel = _autoMocker.CreateInstance<OperationsViewModel>();

            Assert.True(viewModel.RemoveCommand.CanExecute(null));
            viewModel.RemoveCommand.Execute(null);

            await Task.WhenAny(Task.Delay(DelayMs), taskCompletionSource.Task);

            _autoMocker
                .Verify<IOperationsService>(m => m.RemoveAsync(_files),
                    isConfirmed ? Times.Once() : Times.Never());
        }

        [Fact]
        public void TestRemoveNoFiles()
        {
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(new string[0]);
            _autoMocker
                .Setup<IDialogService, Task<RemoveNodesConfirmationDialogResult>>(m =>
                    m.ShowDialogAsync<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>(
                        It.IsAny<string>(), It.IsAny<NodesRemovingNavigationParameter>()))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<OperationsViewModel>();

            Assert.True(viewModel.RemoveCommand.CanExecute(null));
            viewModel.RemoveCommand.Execute(null);

            _autoMocker
                .Verify<IDialogService, Task<RemoveNodesConfirmationDialogResult>>(m =>
                    m.ShowDialogAsync<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>(
                        It.IsAny<string>(), It.IsAny<NodesRemovingNavigationParameter>()), Times.Never);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestMoveToTrash(bool isConfirmed)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            _autoMocker
                .Setup<ITrashCanService>(m => m.MoveToTrashAsync(_files, It.IsAny<CancellationToken>()))
                .Callback(() => taskCompletionSource.SetResult(true))
                .Verifiable();
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(_files);
            _autoMocker
                .Setup<IDialogService, Task<RemoveNodesConfirmationDialogResult>>(m =>
                    m.ShowDialogAsync<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>(
                        nameof(RemoveNodesConfirmationDialogViewModel), It.Is<NodesRemovingNavigationParameter>(p =>
                            p.IsRemovingToTrash && p.Files.Count == _files.Length && p.Files[0] == _files[0] && p.Files[1] == _files[1])))
                .Returns(Task.FromResult(new RemoveNodesConfirmationDialogResult(isConfirmed)));

            var viewModel = _autoMocker.CreateInstance<OperationsViewModel>();

            Assert.True(viewModel.MoveToTrashCommand.CanExecute(null));
            viewModel.MoveToTrashCommand.Execute(null);

            await Task.WhenAny(Task.Delay(DelayMs), taskCompletionSource.Task);

            _autoMocker
                .Verify<ITrashCanService>(m => m.MoveToTrashAsync(_files, It.IsAny<CancellationToken>()),
                    isConfirmed ? Times.Once() : Times.Never());
        }

        [Fact]
        public void TestMoveToTrashNoFiles()
        {
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(new string[0]);
            _autoMocker
                .Setup<IDialogService, Task<RemoveNodesConfirmationDialogResult>>(m =>
                    m.ShowDialogAsync<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>(
                        It.IsAny<string>(), It.IsAny<NodesRemovingNavigationParameter>()))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<OperationsViewModel>();

            Assert.True(viewModel.MoveToTrashCommand.CanExecute(null));
            viewModel.MoveToTrashCommand.Execute(null);

            _autoMocker
                .Verify<IDialogService, Task<RemoveNodesConfirmationDialogResult>>(m =>
                    m.ShowDialogAsync<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>(
                        It.IsAny<string>(), It.IsAny<NodesRemovingNavigationParameter>()), Times.Never);
        }
    }
}