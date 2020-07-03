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

            var filesPanelModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            _autoMocker.Verify<IFilesPanelStateService, PanelModel>(m => m.GetPanelState(), Times.Once);
            _autoMocker.Verify<IDirectoryService, string>(m => m.GetAppRootDirectory(), Times.Once);

            Assert.Single(filesPanelModel.Tabs);
            Assert.Equal(tabViewModel, filesPanelModel.SelectedTab);
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

            var filesPanelModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            _autoMocker.Verify<IFilesPanelStateService, PanelModel>(m => m.GetPanelState(), Times.Once);

            Assert.Equal(tabsCount, filesPanelModel.Tabs.Count());
        }

        [Theory]
        [InlineData(SortingColumn.Date, false)]
        [InlineData(SortingColumn.Extension, true)]
        public async Task TestSetDirectory(SortingColumn sortingColumn, bool isSortingByAscendingEnabled)
        {
            const int saveTimeout = 50;
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

            var filesPanelModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            Assert.Equal(AppRootDirectory, filesPanelModel.CurrentDirectory);
            Assert.Single(filesPanelModel.FileSystemNodes);

            filesPanelModel.CurrentDirectory = NewDirectory;

            Assert.Equal(NewDirectory, filesPanelModel.CurrentDirectory);
            tabViewModelMock.VerifySet(m => m.CurrentDirectory = NewDirectory);

            await Task.Delay(saveTimeout * 3);
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
    }
}