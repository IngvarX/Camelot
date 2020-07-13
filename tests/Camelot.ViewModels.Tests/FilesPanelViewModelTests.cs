using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.DataAccess.Models;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests
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

        [Fact]
        public void TestEmptyFilesPanelState()
        {
            var tabViewModel = new Mock<ITabViewModel>().Object;
            _autoMocker
                .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<TabModel>()))
                .Returns(tabViewModel);
            _autoMocker
                .Setup<IFilesPanelStateService, PanelModel>(m => m.GetPanelState())
                .Returns(PanelModel.Empty)
                .Verifiable();

            _autoMocker
                .Setup<IDirectoryService, string>(m => m.GetAppRootDirectory())
                .Returns(AppRootDirectory)
                .Verifiable();
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
                .Returns(true);

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            _autoMocker.Verify<IFilesPanelStateService, PanelModel>(m => m.GetPanelState(), Times.Once);
            _autoMocker.Verify<IDirectoryService, string>(m => m.GetAppRootDirectory(), Times.Once);

            Assert.Single(filesPanelViewModel.Tabs);
            Assert.Equal(tabViewModel, filesPanelViewModel.SelectedTab);
        }

        [Fact]
        public void TestNonEmptyFilesPanelState()
        {
            var tabsCount = new Random().Next(2, 10);

            var tabViewModel = new Mock<ITabViewModel>().Object;
            _autoMocker
                .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<TabModel>()))
                .Returns(tabViewModel);
            _autoMocker
                .Setup<IFilesPanelStateService, PanelModel>(m => m.GetPanelState())
                .Returns(new PanelModel
                {
                    Tabs = Enumerable
                        .Repeat(new TabModel {Directory = AppRootDirectory}, tabsCount)
                        .ToList()
                })
                .Verifiable();

            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
                .Returns(true);

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            _autoMocker.Verify<IFilesPanelStateService, PanelModel>(m => m.GetPanelState(), Times.Once);

            Assert.Equal(tabsCount, filesPanelViewModel.Tabs.Count());
        }

        [Theory]
        [InlineData(SortingColumn.Date, false)]
        [InlineData(SortingColumn.Extension, true)]
        public async Task TestSetDirectory(SortingColumn sortingColumn, bool isSortingByAscendingEnabled)
        {
            const int saveTimeout = 30;
            _autoMocker.Use(new FilePanelConfiguration {SaveTimeoutMs = saveTimeout});
            var currentDirectory = AppRootDirectory;
            var tabViewModelMock = new Mock<ITabViewModel>();
            tabViewModelMock
                .SetupGet(m => m.CurrentDirectory)
                .Returns(() => currentDirectory);
            tabViewModelMock
                .SetupSet(m => m.CurrentDirectory = NewDirectory)
                .Callback<string>(s => currentDirectory = s)
                .Verifiable();
            var sortingModelMock = new Mock<IFileSystemNodesSortingViewModel>();
            sortingModelMock
                .SetupGet(m => m.SortingColumn)
                .Returns(sortingColumn);
            sortingModelMock
                .SetupGet(m => m.IsSortingByAscendingEnabled)
                .Returns(isSortingByAscendingEnabled);
            tabViewModelMock
                .SetupGet(m => m.SortingViewModel)
                .Returns(sortingModelMock.Object);
            _autoMocker
                .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<TabModel>()))
                .Returns(tabViewModelMock.Object);
            _autoMocker
                .Setup<IFilesPanelStateService, PanelModel>(m => m.GetPanelState())
                .Returns(new PanelModel
                {
                    Tabs = new List<TabModel> {new TabModel {Directory = AppRootDirectory}}
                });
            PanelModel panelModel = null;
            _autoMocker
                .Setup<IFilesPanelStateService>(m => m.SavePanelState(It.IsAny<PanelModel>()))
                .Callback<PanelModel>(pm => panelModel = pm)
                .Verifiable();
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
                .Returns(true);
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(NewDirectory))
                .Returns(true);
            _autoMocker
                .Setup<IDirectoryService, IReadOnlyCollection<DirectoryModel>>(m => m.GetChildDirectories(It.IsAny<string>()))
                .Returns(new DirectoryModel[] {});
            _autoMocker
                .Setup<IFileService, IReadOnlyCollection<FileModel>>(m => m.GetFiles(AppRootDirectory))
                .Returns(new[] {new FileModel {Name = File}});
            _autoMocker
                .Setup<IFileService, IReadOnlyCollection<FileModel>>(m => m.GetFiles(NewDirectory))
                .Returns(new FileModel[] {});

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            Assert.Equal(AppRootDirectory, filesPanelViewModel.CurrentDirectory);
            Assert.Single(filesPanelViewModel.FileSystemNodes);

            filesPanelViewModel.CurrentDirectory = NewDirectory;

            Assert.Equal(NewDirectory, filesPanelViewModel.CurrentDirectory);
            tabViewModelMock.VerifySet(m => m.CurrentDirectory = NewDirectory);

            await Task.Delay(saveTimeout * 10);
            _autoMocker.Verify<IFilesPanelStateService>(m => m.SavePanelState(It.IsAny<PanelModel>()), Times.Once);

            Assert.NotNull(panelModel);
            Assert.Equal(0, panelModel.SelectedTabIndex);
            Assert.Single(panelModel.Tabs);

            var tab = panelModel.Tabs.Single();
            Assert.Equal(NewDirectory, tab.Directory);

            var settings = tab.SortingSettings;
            Assert.Equal(sortingColumn, (SortingColumn) settings.SortingMode);
            Assert.Equal(isSortingByAscendingEnabled, settings.IsAscending);
        }

        [Fact]
        public void TestSelection()
        {
            const long size = 42;
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
            var sortingModelMock = new Mock<IFileSystemNodesSortingViewModel>();
            sortingModelMock
                .SetupGet(m => m.SortingColumn)
                .Returns(SortingColumn.Date);
            sortingModelMock
                .SetupGet(m => m.IsSortingByAscendingEnabled)
                .Returns(false);
            tabViewModelMock
                .SetupGet(m => m.SortingViewModel)
                .Returns(sortingModelMock.Object);
            _autoMocker
                .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<TabModel>()))
                .Returns(tabViewModelMock.Object);
            _autoMocker
                .Setup<IFileSystemNodeViewModelComparerFactory, IComparer<IFileSystemNodeViewModel>>(m => m.Create(It.IsAny<IFileSystemNodesSortingViewModel>()))
                .Returns(new Mock<IComparer<IFileSystemNodeViewModel>>().Object);
            _autoMocker
                .Setup<IFilesPanelStateService, PanelModel>(m => m.GetPanelState())
                .Returns(new PanelModel
                {
                    Tabs = new List<TabModel> {new TabModel {Directory = AppRootDirectory}}
                });
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
                .Returns(true);
            _autoMocker
                .Setup<IDirectoryService, IReadOnlyCollection<DirectoryModel>>(m =>
                    m.GetChildDirectories(It.IsAny<string>()))
                .Returns(new[] {new DirectoryModel {Name = NewDirectory}});
            _autoMocker
                .Setup<IFileService, IReadOnlyCollection<FileModel>>(m => m.GetFiles(AppRootDirectory))
                .Returns(new[] {new FileModel {Name = File}});
            const string formattedSize = "42";
            _autoMocker
                .Setup<IFileSizeFormatter, string>(m => m.GetFormattedSize(size))
                .Returns(formattedSize);

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();
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
                .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<TabModel>()))
                .Returns(tabViewModelMock.Object);
            _autoMocker
                .Setup<IFileSystemNodeViewModelComparerFactory, IComparer<IFileSystemNodeViewModel>>(m => m.Create(It.IsAny<IFileSystemNodesSortingViewModel>()))
                .Returns(new Mock<IComparer<IFileSystemNodeViewModel>>().Object);
            _autoMocker
                .Setup<IFilesPanelStateService, PanelModel>(m => m.GetPanelState())
                .Returns(new PanelModel
                {
                    Tabs = new List<TabModel> {new TabModel {Directory = AppRootDirectory}}
                });
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
                .Returns(true);
            _autoMocker
                .Setup<IDirectoryService, IReadOnlyCollection<DirectoryModel>>(m =>
                    m.GetChildDirectories(It.IsAny<string>()))
                .Returns(new[] {new DirectoryModel {Name = NewDirectory}});
            _autoMocker
                .Setup<IFileService, IReadOnlyCollection<FileModel>>(m => m.GetFiles(AppRootDirectory))
                .Returns(new[] {new FileModel {Name = File}});

            var filesPanelViewModel = _autoMocker.CreateInstance<FilesPanelViewModel>();
            var isActivationCallbackCalled = false;
            filesPanelViewModel.ActivatedEvent += (sender, args) => isActivationCallbackCalled = true;

            Assert.True(filesPanelViewModel.ActivateCommand.CanExecute(null));
            filesPanelViewModel.ActivateCommand.Execute(null);
            Assert.True(isActivationCallbackCalled);
            tabViewModelMock.VerifySet(m => m.IsGloballyActive = true, Times.AtLeastOnce);

            var isDeactivationCallbackCalled = false;
            filesPanelViewModel.DeactivatedEvent += (sender, args) => isDeactivationCallbackCalled = true;
            filesPanelViewModel.Deactivate();
            Assert.True(isDeactivationCallbackCalled);
            tabViewModelMock.VerifySet(m => m.IsGloballyActive = false, Times.Once);
            Assert.Empty(filesPanelViewModel.SelectedFileSystemNodes);
        }
    }
}