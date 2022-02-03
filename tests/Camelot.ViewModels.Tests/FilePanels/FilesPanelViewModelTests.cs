using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.RecursiveSearch;
using Camelot.Services.Abstractions.Specifications;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.Operations;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels;

public class FilesPanelViewModelTests
{
    private const string AppRootDirectory = "Root";
    private const string ParentDirectory = "Parent";
    private const string NewDirectory = "New";
    private const string File = "File";

    private readonly AutoMocker _autoMocker;

    public FilesPanelViewModelTests()
    {
        _autoMocker = new AutoMocker();

        _autoMocker
            .Setup<ISearchViewModel, INodeSpecification>(m => m.GetSpecification())
            .Returns(new Mock<INodeSpecification>().Object);
        _autoMocker
            .Setup<IDirectorySelectorViewModel, string>(m => m.CurrentDirectory)
            .Returns(AppRootDirectory);
        _autoMocker
            .Setup<IDirectoryService, IReadOnlyList<DirectoryModel>>(m => m.GetChildDirectories(It.IsAny<string>(), It.IsAny<ISpecification<DirectoryModel>>()))
            .Returns(new DirectoryModel[] {});
        _autoMocker
            .Setup<IFileService, IReadOnlyList<FileModel>>(m =>
                m.GetFiles(It.IsAny<string>(), It.IsAny<ISpecification<NodeModelBase>>()))
            .Returns(new FileModel[] { });
        _autoMocker
            .Setup<IPermissionsService, bool>(m => m.CheckIfHasAccess(It.IsAny<string>()))
            .Returns(true);
    }

    [Fact]
    public void TestProperties()
    {
        var searchViewModel = _autoMocker.GetMock<ISearchViewModel>().Object;
        var tabsListViewModel = _autoMocker.GetMock<ITabsListViewModel>().Object;
        var operationsViewModel = _autoMocker.GetMock<IOperationsViewModel>().Object;
        var directorySelectorViewModel = _autoMocker.GetMock<IDirectorySelectorViewModel>().Object;
        var dragAndDropOperationsMediator = _autoMocker.GetMock<IDragAndDropOperationsMediator>().Object;
        var clipboardOperationsViewModel = _autoMocker.GetMock<IClipboardOperationsViewModel>().Object;

        var viewModel = new FilesPanelViewModel(
            _autoMocker.GetMock<IFileService>().Object,
            _autoMocker.GetMock<IDirectoryService>().Object,
            _autoMocker.GetMock<INodesSelectionService>().Object,
            _autoMocker.GetMock<INodeService>().Object,
            _autoMocker.GetMock<IFileSystemNodeViewModelFactory>().Object,
            _autoMocker.GetMock<IFileSystemWatchingService>().Object,
            _autoMocker.GetMock<IApplicationDispatcher>().Object,
            _autoMocker.GetMock<IFileSizeFormatter>().Object,
            _autoMocker.GetMock<IFileSystemNodeViewModelComparerFactory>().Object,
            _autoMocker.GetMock<IRecursiveSearchService>().Object,
            _autoMocker.GetMock<IFilePanelDirectoryObserver>().Object,
            _autoMocker.GetMock<IPermissionsService>().Object,
            _autoMocker.GetMock<IDialogService>().Object,
            searchViewModel,
            tabsListViewModel,
            operationsViewModel,
            directorySelectorViewModel,
            dragAndDropOperationsMediator,
            clipboardOperationsViewModel
        );

        Assert.Equal(searchViewModel, viewModel.SearchViewModel);
        Assert.Equal(tabsListViewModel, viewModel.TabsListViewModel);
        Assert.Equal(operationsViewModel, viewModel.OperationsViewModel);
        Assert.Equal(directorySelectorViewModel, viewModel.DirectorySelectorViewModel);
        Assert.Equal(dragAndDropOperationsMediator, viewModel.DragAndDropOperationsMediator);
        Assert.Equal(clipboardOperationsViewModel, viewModel.ClipboardOperationsViewModel);
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
            .GetMock<IFilePanelDirectoryObserver>()
            .SetupSet<string>(m => m.CurrentDirectory = AppRootDirectory)
            .Verifiable();
        _autoMocker
            .GetMock<IFilePanelDirectoryObserver>()
            .SetupSet<string>(m => m.CurrentDirectory = NewDirectory)
            .Verifiable();

        var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

        filesPanelViewModel.CurrentDirectory = NewDirectory;

        _autoMocker
            .GetMock<IFilePanelDirectoryObserver>()
            .VerifySet(m => m.CurrentDirectory = NewDirectory,
                Times.Once);
    }

    [Fact]
    public void TestSelectedTabChanged()
    {
        var tabViewModelMock = new Mock<ITabViewModel>();
        _autoMocker
            .Setup<ITabsListViewModel, ITabViewModel>(m => m.SelectedTab)
            .Returns(tabViewModelMock.Object);

        var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

        var isCallbackCalled = false;
        filesPanelViewModel.Activated += (sender, args) => isCallbackCalled = true;

        tabViewModelMock
            .VerifySet(m => m.IsGloballyActive = true,
                Times.Once);

        _autoMocker
            .GetMock<ITabsListViewModel>()
            .Raise(m => m.SelectedTabChanged += null, EventArgs.Empty);

        Assert.True(isCallbackCalled);

        tabViewModelMock
            .VerifySet(m => m.IsGloballyActive = true,
                Times.Exactly(2));
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
            .Callback<string>(s => currentDirectory = s);
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
            .Verify<IDirectoryService, DirectoryModel>(m => m.GetParentDirectory(NewDirectory),
                Times.Exactly(3));
        _autoMocker
            .GetMock<IFilePanelDirectoryObserver>()
            .Raise(m => m.CurrentDirectoryChanged += null, EventArgs.Empty);

        Assert.Single(filesPanelViewModel.FileSystemNodes);
        Assert.Equal(NewDirectory, filesPanelViewModel.CurrentDirectory);

        _autoMocker
            .Verify<IDirectoryService, DirectoryModel>(m => m.GetParentDirectory(NewDirectory),
                Times.Exactly(6));
    }

    [Fact]
    public void TestDirectoryUpdatedNoAccess()
    {
        var currentDirectory = AppRootDirectory;
        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.CurrentDirectory)
            .Returns(() => currentDirectory);
        tabViewModelMock
            .SetupSet(m => m.CurrentDirectory = NewDirectory)
            .Callback<string>(s => currentDirectory = s);
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
            .Setup<ISearchViewModel, INodeSpecification>(m => m.GetSpecification())
            .Returns(new Mock<INodeSpecification>().Object);
        _autoMocker
            .Setup<IFilePanelDirectoryObserver, string>(m => m.CurrentDirectory)
            .Returns(AppRootDirectory);
        _autoMocker
            .Setup<IPermissionsService, bool>(m => m.CheckIfHasAccess(It.IsAny<string>()))
            .Returns<string>(s => s != NewDirectory);
        _autoMocker
            .Setup<IDialogService, Task>(m => m.ShowDialogAsync(nameof(AccessDeniedDialogViewModel),
                It.Is<AccessDeniedNavigationParameter>(p => p.Directory == NewDirectory)))
            .Verifiable();

        var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

        _autoMocker
            .Verify<IDirectoryService, DirectoryModel>(m => m.GetParentDirectory(NewDirectory),
                Times.Never);
        _autoMocker
            .Setup<IFilePanelDirectoryObserver, string>(m => m.CurrentDirectory)
            .Returns(NewDirectory);
        _autoMocker
            .GetMock<IFilePanelDirectoryObserver>()
            .Raise(m => m.CurrentDirectoryChanged += null, EventArgs.Empty);

        Assert.Equal(AppRootDirectory, filesPanelViewModel.CurrentDirectory);

        _autoMocker
            .Verify<IDialogService, Task>(m => m.ShowDialogAsync(nameof(AccessDeniedDialogViewModel),
                    It.Is<AccessDeniedNavigationParameter>(p => p.Directory == NewDirectory)),
                Times.Once);
        _autoMocker
            .GetMock<IFilePanelDirectoryObserver>()
            .VerifySet(m => m.CurrentDirectory = AppRootDirectory,
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
            .Verify<IDirectoryService, IReadOnlyList<DirectoryModel>>(
                m => m.GetChildDirectories(AppRootDirectory, It.IsAny<ISpecification<DirectoryModel>>()),
                Times.Once);
        _autoMocker
            .Verify<IFileService, IReadOnlyList<FileModel>>(
                m => m.GetFiles(AppRootDirectory, It.IsAny<ISpecification<NodeModelBase>>()),
                Times.Once);
        _autoMocker
            .GetMock<IFilePanelDirectoryObserver>()
            .Raise(m => m.CurrentDirectoryChanged += null, EventArgs.Empty);
        _autoMocker
            .Verify<IDirectoryService, IReadOnlyList<DirectoryModel>>(
                m => m.GetChildDirectories(AppRootDirectory, It.IsAny<ISpecification<DirectoryModel>>()),
                Times.Exactly(2));
        _autoMocker
            .Verify<IFileService, IReadOnlyList<FileModel>>(
                m => m.GetFiles(AppRootDirectory, It.IsAny<ISpecification<NodeModelBase>>()),
                Times.Exactly(2));

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
                Times.Exactly(refreshCount + 2));
        _autoMocker
            .Verify<IFileService, IReadOnlyList<FileModel>>(
                m => m.GetFiles(AppRootDirectory, It.IsAny<ISpecification<NodeModelBase>>()),
                Times.Exactly(refreshCount + 2));
    }

    [Fact]
    public void TestGoToParentDirectoryCommandNoDirectory()
    {
        _autoMocker
            .Setup<IFilePanelDirectoryObserver, string>(m => m.CurrentDirectory)
            .Returns(AppRootDirectory);
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<ITabsListViewModel, ITabViewModel>(m => m.SelectedTab)
            .Returns(new Mock<ITabViewModel>().Object);

        var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

        Assert.False(filesPanelViewModel.GoToParentDirectoryCommand.CanExecute(null));
    }

    [Fact]
    public void TestGoToParentDirectoryCommand()
    {
        _autoMocker
            .Setup<IFilePanelDirectoryObserver, string>(m => m.CurrentDirectory)
            .Returns(AppRootDirectory);
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<ITabsListViewModel, ITabViewModel>(m => m.SelectedTab)
            .Returns(new Mock<ITabViewModel>().Object);

        var model = new DirectoryModel
        {
            FullPath = ParentDirectory
        };
        _autoMocker
            .Setup<IDirectoryService, DirectoryModel>(m => m.GetParentDirectory(AppRootDirectory))
            .Returns(model);

        var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

        Assert.True(filesPanelViewModel.GoToParentDirectoryCommand.CanExecute(null));
        filesPanelViewModel.GoToParentDirectoryCommand.Execute(null);

        _autoMocker
            .GetMock<IFilePanelDirectoryObserver>()
            .VerifySet(m => m.CurrentDirectory = ParentDirectory);
    }
}