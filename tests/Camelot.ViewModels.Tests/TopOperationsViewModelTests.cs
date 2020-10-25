using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class TopOperationsViewModelTests
    {
        private const string Directory = "Directory";
        private const string FileToArchive = "FileToArchive";
        private const string DirectoryToArchive = "DirectoryToArchive";
        private const string ArchivePath = "ArchivePath";
        private const string NewArchivePath = "NewArchivePath";

        private readonly AutoMocker _autoMocker;

        public TopOperationsViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestPackCommand()
        {
            const ArchiveType archiveType = ArchiveType.TarGz;
            var nodes = new[] {FileToArchive, DirectoryToArchive};
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(nodes);
            _autoMocker
                .Setup<IDirectoryService, string>(m => m.SelectedDirectory)
                .Returns(Directory);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(Directory, FileToArchive))
                .Returns(ArchivePath);
            _autoMocker
                .Setup<IDialogService, Task<CreateArchiveDialogResult>>(m => m.ShowDialogAsync<CreateArchiveDialogResult, CreateArchiveNavigationParameter>(
                    nameof(CreateArchiveDialogViewModel), It.Is<CreateArchiveNavigationParameter>(p =>
                        !p.IsPackingSingleFile && p.DefaultArchivePath == ArchivePath)))
                .ReturnsAsync(new CreateArchiveDialogResult(NewArchivePath, archiveType));
            _autoMocker
                .Setup<IArchiveService>(m => m.PackAsync(
                    It.Is<IReadOnlyList<string>>(l =>
                        l.Count == nodes.Length
                        && l[0] == FileToArchive
                        && l[1] == DirectoryToArchive), NewArchivePath, archiveType))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<TopOperationsViewModel>();

            Assert.True(viewModel.PackCommand.CanExecute(null));
            viewModel.PackCommand.Execute(null);

            _autoMocker
                .Verify<IArchiveService>(m => m.PackAsync(
                    It.Is<IReadOnlyList<string>>(l =>
                            l.Count == nodes.Length
                            && l[0] == FileToArchive
                            && l[1] == DirectoryToArchive), NewArchivePath, archiveType), Times.Once);
        }

        [Fact]
        public void TestPackCommandNoFiles()
        {
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(new List<string>());
            _autoMocker
                .Setup<IDialogService, Task<CreateArchiveDialogResult>>(m => m.ShowDialogAsync<CreateArchiveDialogResult, CreateArchiveNavigationParameter>(
                    nameof(CreateArchiveDialogViewModel), It.IsAny<CreateArchiveNavigationParameter>()))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<TopOperationsViewModel>();

            Assert.True(viewModel.PackCommand.CanExecute(null));
            viewModel.PackCommand.Execute(null);

            _autoMocker
                .Verify<IDialogService, Task<CreateArchiveDialogResult>>(m => m.ShowDialogAsync<CreateArchiveDialogResult, CreateArchiveNavigationParameter>(
                    nameof(CreateArchiveDialogViewModel), It.IsAny<CreateArchiveNavigationParameter>()),
                    Times.Never);
        }

        [Fact]
        public void TestPackCommandCancel()
        {
            var nodes = new[] {FileToArchive, DirectoryToArchive};
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(nodes);
            _autoMocker
                .Setup<IDirectoryService, string>(m => m.SelectedDirectory)
                .Returns(Directory);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(Directory, FileToArchive))
                .Returns(ArchivePath);
            _autoMocker
                .Setup<IDialogService, Task<CreateArchiveDialogResult>>(m => m.ShowDialogAsync<CreateArchiveDialogResult, CreateArchiveNavigationParameter>(
                    nameof(CreateArchiveDialogViewModel), It.Is<CreateArchiveNavigationParameter>(p =>
                        !p.IsPackingSingleFile && p.DefaultArchivePath == ArchivePath)))
                .ReturnsAsync((CreateArchiveDialogResult) null);
            _autoMocker
                .Setup<IArchiveService>(m => m.PackAsync(
                    It.IsAny<IReadOnlyList<string>>(), It.IsAny<string>(), It.IsAny<ArchiveType>()))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<TopOperationsViewModel>();

            Assert.True(viewModel.PackCommand.CanExecute(null));
            viewModel.PackCommand.Execute(null);

            _autoMocker
                .Verify<IArchiveService>(m => m.PackAsync(
                    It.IsAny<IReadOnlyList<string>>(), It.IsAny<string>(), It.IsAny<ArchiveType>()),
                    Times.Never);
        }

        [Fact]
        public void TestExtractCommandNoFiles()
        {
            var nodes = new[] {FileToArchive, DirectoryToArchive};
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(nodes);
            _autoMocker
                .Setup<ISystemDialogService>(m => m.GetDirectoryAsync(null))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<TopOperationsViewModel>();

            Assert.True(viewModel.ExtractCommand.CanExecute(null));
            viewModel.ExtractCommand.Execute(null);

            _autoMocker
                .Verify<ISystemDialogService>(m => m.GetDirectoryAsync(null),
                    Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void TestExtractCommandCancel(string directory)
        {
            var nodes = new[] {FileToArchive, DirectoryToArchive};
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(nodes);
            _autoMocker
                .Setup<IArchiveService, bool>(m => m.CheckIfNodeIsArchive(FileToArchive))
                .Returns(true);
            _autoMocker
                .Setup<ISystemDialogService, Task<string>>(m => m.GetDirectoryAsync(null))
                .ReturnsAsync(directory);

            var viewModel = _autoMocker.CreateInstance<TopOperationsViewModel>();

            Assert.True(viewModel.ExtractCommand.CanExecute(null));
            viewModel.ExtractCommand.Execute(null);

            _autoMocker
                .Verify<IArchiveService>(m => m.ExtractAsync(It.IsAny<string>(),
                        It.IsAny<string>()),
                    Times.Never);
        }

        [Fact]
        public void TestExtractCommand()
        {
            var nodes = new[] {FileToArchive, DirectoryToArchive};
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(nodes);
            _autoMocker
                .Setup<IArchiveService, bool>(m => m.CheckIfNodeIsArchive(FileToArchive))
                .Returns(true);
            _autoMocker
                .Setup<ISystemDialogService, Task<string>>(m => m.GetDirectoryAsync(null))
                .ReturnsAsync(Directory);
            _autoMocker
                .Setup<IArchiveService>(m => m.ExtractAsync(FileToArchive,Directory))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<TopOperationsViewModel>();

            Assert.True(viewModel.ExtractCommand.CanExecute(null));
            viewModel.ExtractCommand.Execute(null);

            _autoMocker
                .Verify<IArchiveService>(m => m.ExtractAsync(FileToArchive, Directory),
                    Times.Once);
        }

        [Fact]
        public void TestOpenTerminalCommand()
        {
            _autoMocker
                .Setup<ITerminalService>(m => m.Open(Directory))
                .Verifiable();
            _autoMocker
                .Setup<IDirectoryService, string>(m => m.SelectedDirectory)
                .Returns(Directory);

            var viewModel = _autoMocker.CreateInstance<TopOperationsViewModel>();

            Assert.True(viewModel.OpenTerminalCommand.CanExecute(null));
            viewModel.OpenTerminalCommand.Execute(null);

            _autoMocker.Verify<ITerminalService>(m => m.Open(Directory), Times.Once);
        }

        [Fact]
        public void TestSearchCommand()
        {
            _autoMocker
                .Setup<IFilesOperationsMediator>(m => m.ToggleSearchPanelVisibility())
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<TopOperationsViewModel>();

            Assert.True(viewModel.SearchCommand.CanExecute(null));
            viewModel.SearchCommand.Execute(null);

            _autoMocker
                .Verify<IFilesOperationsMediator>(m => m.ToggleSearchPanelVisibility(), Times.Once);
        }
    }
}