using System;
using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels
{
    public class FilesPanelViewModelTests
    {
        private const string AppRootDirectory = "Root";
        private const string NewDirectory = "New";
        private const string File = "File";

        private readonly AutoMocker _autoMocker;

        public FilesPanelViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestIsActive(bool isTabActive)
        {
            var tabViewModelMock = new Mock<ITabViewModel>();
            tabViewModelMock
                .SetupGet(m => m.IsGloballyActive)
                .Returns(isTabActive);
            _autoMocker
                .Setup<ITabsListViewModel, ITabViewModel>(m => m.SelectedTab)
                .Returns(tabViewModelMock.Object);

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            Assert.Equal(isTabActive, filesPanelViewModel.IsActive);
        }

        [Fact]
        public void TestSetDirectory()
        {
            var tabViewModelMock = new Mock<ITabViewModel>();
            tabViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(AppRootDirectory);
            _autoMocker
                .Setup<ITabsListViewModel, ITabViewModel>(m => m.SelectedTab)
                .Returns(tabViewModelMock.Object);
            _autoMocker
                .GetMock<IDirectorySelectorViewModel>()
                .SetupSet<string>(m => m.CurrentDirectory = AppRootDirectory)
                .Verifiable();
            _autoMocker
                .GetMock<IDirectorySelectorViewModel>()
                .SetupSet<string>(m => m.CurrentDirectory = NewDirectory)
                .Verifiable();

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            _autoMocker
                .GetMock<IDirectorySelectorViewModel>()
                .VerifySet(m => m.CurrentDirectory = AppRootDirectory,
                    Times.Once);

            filesPanelViewModel.CurrentDirectory = NewDirectory;

            _autoMocker
                .GetMock<IDirectorySelectorViewModel>()
                .VerifySet(m => m.CurrentDirectory = NewDirectory,
                    Times.Once);
        }

        [Fact]
        public void TestSelectedTabChanged()
        {
            var tabViewModelMock = new Mock<ITabViewModel>();
            tabViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(AppRootDirectory);
            _autoMocker
                .Setup<ITabsListViewModel, ITabViewModel>(m => m.SelectedTab)
                .Returns(tabViewModelMock.Object);
            _autoMocker
                .GetMock<IDirectorySelectorViewModel>()
                .SetupSet<string>(m => m.CurrentDirectory = AppRootDirectory)
                .Verifiable();

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            _autoMocker
                .GetMock<IDirectorySelectorViewModel>()
                .VerifySet(m => m.CurrentDirectory = AppRootDirectory,
                    Times.Once);

            _autoMocker
                .GetMock<ITabsListViewModel>()
                .Raise(m => m.SelectedTabChanged += null, EventArgs.Empty);

            Assert.Equal(tabViewModelMock.Object, filesPanelViewModel.SelectedTab);

            _autoMocker
                .GetMock<IDirectorySelectorViewModel>()
                .VerifySet(m => m.CurrentDirectory = AppRootDirectory,
                    Times.Exactly(2));
        }

        [Fact]
        public void TestCopyToClipboardCommand()
        {
            _autoMocker
                .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
                .Returns(new[] {File});
            _autoMocker
                .Setup<IClipboardOperationsService>(m => m.CopyFilesAsync(
                    It.Is<IReadOnlyList<string>>(l => l.Single() == File)))
                .Verifiable();
            _autoMocker
                .Setup<ITabsListViewModel, ITabViewModel>(m => m.SelectedTab)
                .Returns(Mock.Of<ITabViewModel>());

           var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

           Assert.True(filesPanelViewModel.CopyToClipboardCommand.CanExecute(null));

            filesPanelViewModel.CopyToClipboardCommand.Execute(null);

            _autoMocker
                .Verify<IClipboardOperationsService>(m => m.CopyFilesAsync(
                    It.Is<IReadOnlyList<string>>(l => l.Single() == File)),
                    Times.Once);
        }

        [Fact]
        public void TestPasteFromClipboardCommand()
        {
            var tabViewModelMock = new Mock<ITabViewModel>();
            tabViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(AppRootDirectory);
            _autoMocker
                .Setup<ITabsListViewModel, ITabViewModel>(m => m.SelectedTab)
                .Returns(tabViewModelMock.Object);
            _autoMocker
                .Setup<IFilePanelDirectoryObserver, string>(m => m.CurrentDirectory)
                .Returns(AppRootDirectory);
            _autoMocker
                .Setup<ISearchViewModel, INodeSpecification>(m => m.GetSpecification())
                .Returns(new Mock<INodeSpecification>().Object);
            _autoMocker
                .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m => m.GetChildDirectories(It.IsAny<string>(), It.IsAny<ISpecification<DirectoryModel>>()))
                .Returns(new DirectoryModel[] {});
            _autoMocker
                .Setup<IFileService, IReadOnlyList<FileModel>>(m =>
                    m.GetFiles(It.IsAny<string>(), It.IsAny<ISpecification<NodeModelBase>>()))
                .Returns(new FileModel[] { });

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            _autoMocker
                .GetMock<IFilePanelDirectoryObserver>()
                .Raise(m => m.CurrentDirectoryChanged += null, EventArgs.Empty);

            Assert.True(filesPanelViewModel.PasteFromClipboardCommand.CanExecute(null));

            filesPanelViewModel.PasteFromClipboardCommand.Execute(null);

            _autoMocker
                .Verify<IClipboardOperationsService>(m => m.PasteFilesAsync(AppRootDirectory),
                    Times.Once);
        }

        [Fact]
        public void TestDirectoryUpdated()
        {
            var currentDirectory = AppRootDirectory;
            var tabViewModelMock = new Mock<ITabViewModel>();
            tabViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(() => currentDirectory);
            tabViewModelMock
                .SetupSet(m => m.CurrentDirectory = NewDirectory)
                .Callback<string>(s => currentDirectory = s)
                .Verifiable();
            _autoMocker
                .Setup<ITabsListViewModel, ITabViewModel>(m => m.SelectedTab)
                .Returns(tabViewModelMock.Object);
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(NewDirectory))
                .Returns(true);
            _autoMocker
                .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m => m.GetChildDirectories(It.IsAny<string>(), It.IsAny<ISpecification<DirectoryModel>>()))
                .Returns(new DirectoryModel[] {});
            _autoMocker
                .Setup<IFileService, IReadOnlyList<FileModel>>(m => m.GetFiles(NewDirectory, It.IsAny<ISpecification<NodeModelBase>>()))
                .Returns(new[] {new FileModel {Name = File}});
            _autoMocker
                .Setup<ISearchViewModel, INodeSpecification>(m => m.GetSpecification())
                .Returns(new Mock<INodeSpecification>().Object);
            _autoMocker
                .Setup<IFilePanelDirectoryObserver, string>(m => m.CurrentDirectory)
                .Returns(NewDirectory);

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            _autoMocker
                .GetMock<IFilePanelDirectoryObserver>()
                .Raise(m => m.CurrentDirectoryChanged += null, EventArgs.Empty);

            Assert.Single(filesPanelViewModel.FileSystemNodes);
            Assert.Equal(NewDirectory, filesPanelViewModel.CurrentDirectory);
            tabViewModelMock.VerifySet(m => m.CurrentDirectory = NewDirectory);

            _autoMocker
                .Verify<IDirectoryService, DirectoryModel>(m => m.GetParentDirectory(NewDirectory),
                    Times.Once);
        }

        [Fact]
        public void TestSelection()
        {
            const long size = 42;
            _autoMocker
                .Setup<ISearchViewModel, INodeSpecification>(m => m.GetSpecification())
                .Returns(new Mock<INodeSpecification>().Object);
            var fileViewModelMock = new Mock<IFileViewModel>();
            fileViewModelMock
                .SetupGet(m => m.Size)
                .Returns(size);
            _autoMocker
                .Setup<IFileSystemNodeViewModelFactory, IFileSystemNodeViewModel>(m => m.Create(It.IsAny<FileModel>()))
                .Returns(fileViewModelMock.Object);
            _autoMocker
                .Setup<IFileSystemNodeViewModelFactory, IFileSystemNodeViewModel>(m => m.Create(It.IsAny<DirectoryModel>(), It.IsAny<bool>()))
                .Returns(new Mock<IDirectoryViewModel>().Object);
            var tabViewModelMock = new Mock<ITabViewModel>();
            tabViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(AppRootDirectory);
            _autoMocker
                .Setup<ITabsListViewModel, ITabViewModel>(m => m.SelectedTab)
                .Returns(tabViewModelMock.Object);
            _autoMocker
                .Setup<IFileSystemNodeViewModelComparerFactory, IComparer<IFileSystemNodeViewModel>>(m => m.Create(It.IsAny<IFileSystemNodesSortingViewModel>()))
                .Returns(new Mock<IComparer<IFileSystemNodeViewModel>>().Object);
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
                .Returns(true);
            _autoMocker
                .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m =>
                    m.GetChildDirectories(It.IsAny<string>(), It.IsAny<ISpecification<DirectoryModel>>()))
                .Returns(new[] {new DirectoryModel {Name = NewDirectory}});
            _autoMocker
                .Setup<IFileService, IReadOnlyList<FileModel>>(m => m.GetFiles(AppRootDirectory, It.IsAny<ISpecification<NodeModelBase>>()))
                .Returns(new[] {new FileModel {Name = File}});
            const string formattedSize = "42";
            _autoMocker
                .Setup<IFileSizeFormatter, string>(m => m.GetFormattedSize(size))
                .Returns(formattedSize);

            _autoMocker
                .Setup<IFilePanelDirectoryObserver, string>(m => m.CurrentDirectory)
                .Returns(AppRootDirectory);

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            _autoMocker
                .GetMock<IFilePanelDirectoryObserver>()
                .Raise(m => m.CurrentDirectoryChanged += null, EventArgs.Empty);
            Assert.Equal(2, filesPanelViewModel.FileSystemNodes.Count());

            void CheckSelection(int selectedFilesCount, int selectedDirsCount)
            {
                var totalSelectedCount = selectedDirsCount + selectedFilesCount;
                Assert.Equal(totalSelectedCount, filesPanelViewModel.SelectedFileSystemNodes.Count);
                Assert.Equal(totalSelectedCount > 0, filesPanelViewModel.AreAnyFileSystemNodesSelected);
                Assert.Equal(selectedFilesCount, filesPanelViewModel.SelectedFilesCount);
                Assert.Equal(selectedDirsCount, filesPanelViewModel.SelectedDirectoriesCount);
            }

            CheckSelection(0, 0);

            filesPanelViewModel.SelectedFileSystemNodes.Add(filesPanelViewModel.FileSystemNodes.First());
            CheckSelection(0, 1);

            filesPanelViewModel.SelectedFileSystemNodes.Add(filesPanelViewModel.FileSystemNodes.Last());
            CheckSelection(1, 1);

            Assert.Equal(formattedSize, filesPanelViewModel.SelectedFilesSize);

            filesPanelViewModel.SelectedFileSystemNodes.RemoveAt(0);
            CheckSelection(1, 0);

            filesPanelViewModel.SelectedFileSystemNodes.RemoveAt(0);
            CheckSelection(0, 0);
        }

        [Fact]
        public void TestActivationAndDeactivation()
        {
            _autoMocker
                .Setup<IFileSystemNodeViewModelFactory, IFileSystemNodeViewModel>(m => m.Create(It.IsAny<DirectoryModel>(), It.IsAny<bool>()))
                .Returns(new Mock<IDirectoryViewModel>().Object);
            _autoMocker
                .Setup<ISearchViewModel, INodeSpecification>(m => m.GetSpecification())
                .Returns(new Mock<INodeSpecification>().Object);
            var tabViewModelMock = new Mock<ITabViewModel>();
            tabViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(AppRootDirectory);
            tabViewModelMock
                .SetupSet(m => m.IsGloballyActive = true)
                .Verifiable();
            tabViewModelMock
                .SetupSet(m => m.IsGloballyActive = false)
                .Verifiable();
            _autoMocker
                .Setup<ITabsListViewModel, ITabViewModel>(m => m.SelectedTab)
                .Returns(tabViewModelMock.Object);
            _autoMocker
                .Setup<IFileSystemNodeViewModelComparerFactory, IComparer<IFileSystemNodeViewModel>>(m => m.Create(It.IsAny<IFileSystemNodesSortingViewModel>()))
                .Returns(new Mock<IComparer<IFileSystemNodeViewModel>>().Object);
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
                .Returns(true);
            _autoMocker
                .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m =>
                    m.GetChildDirectories(It.IsAny<string>(), It.IsAny<ISpecification<DirectoryModel>>()))
                .Returns(new[] {new DirectoryModel {Name = NewDirectory}});
            _autoMocker
                .Setup<IFileService, IReadOnlyList<FileModel>>(m => m.GetFiles(AppRootDirectory, It.IsAny<ISpecification<NodeModelBase>>()))
                .Returns(new[] {new FileModel {Name = File}});

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();
            var isActivationCallbackCalled = false;
            filesPanelViewModel.Activated += (sender, args) => isActivationCallbackCalled = true;

            Assert.True(filesPanelViewModel.ActivateCommand.CanExecute(null));
            filesPanelViewModel.ActivateCommand.Execute(null);
            Assert.True(isActivationCallbackCalled);
            tabViewModelMock.VerifySet(m => m.IsGloballyActive = true, Times.AtLeastOnce);

            var isDeactivationCallbackCalled = false;
            filesPanelViewModel.Deactivated += (sender, args) => isDeactivationCallbackCalled = true;
            filesPanelViewModel.Deactivate();
            Assert.True(isDeactivationCallbackCalled);
            tabViewModelMock.VerifySet(m => m.IsGloballyActive = false, Times.Once);
            Assert.Empty(filesPanelViewModel.SelectedFileSystemNodes);
        }

        [Fact]
        public void TestRefreshCommand()
        {
            _autoMocker
                .Setup<ISearchViewModel, INodeSpecification>(m => m.GetSpecification())
                .Returns(new Mock<INodeSpecification>().Object);
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
                .Returns(true);
            _autoMocker
                .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m => m.GetChildDirectories(AppRootDirectory, It.IsAny<ISpecification<DirectoryModel>>()))
                .Returns(new DirectoryModel[] {})
                .Verifiable();
            _autoMocker
                .Setup<IFileService, IReadOnlyList<FileModel>>(m => m.GetFiles(AppRootDirectory, It.IsAny<ISpecification<NodeModelBase>>()))
                .Returns(new FileModel[] {})
                .Verifiable();
            var tabViewModelMock = new Mock<ITabViewModel>();
            tabViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(AppRootDirectory);
            _autoMocker
                .Setup<ITabsListViewModel, ITabViewModel>(m => m.SelectedTab)
                .Returns(tabViewModelMock.Object);
            _autoMocker
                .Setup<IFilePanelDirectoryObserver, string>(m => m.CurrentDirectory)
                .Returns(AppRootDirectory);

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            _autoMocker
                .GetMock<IFilePanelDirectoryObserver>()
                .Raise(m => m.CurrentDirectoryChanged += null, EventArgs.Empty);
            _autoMocker
                .Verify<IDirectoryService, IReadOnlyList<DirectoryModel>>(
                    m => m.GetChildDirectories(AppRootDirectory, It.IsAny<ISpecification<DirectoryModel>>()),
                    Times.Once);
            _autoMocker
                .Verify<IFileService, IReadOnlyList<FileModel>>(
                    m => m.GetFiles(AppRootDirectory, It.IsAny<ISpecification<NodeModelBase>>()),
                    Times.Once);

            var random = new Random();
            var refreshCount = random.Next(5, 15);
            for (var i = 0; i < refreshCount; i++)
            {
                Assert.True(filesPanelViewModel.RefreshCommand.CanExecute(null));
                filesPanelViewModel.RefreshCommand.Execute(null);
            }

            _autoMocker
                .Verify<IDirectoryService, IReadOnlyList<DirectoryModel>>(
                    m => m.GetChildDirectories(AppRootDirectory, It.IsAny<ISpecification<DirectoryModel>>()),
                    Times.Exactly(refreshCount + 1));
            _autoMocker
                .Verify<IFileService, IReadOnlyList<FileModel>>(
                    m => m.GetFiles(AppRootDirectory, It.IsAny<ISpecification<NodeModelBase>>()),
                    Times.Exactly(refreshCount + 1));
        }
    }
}