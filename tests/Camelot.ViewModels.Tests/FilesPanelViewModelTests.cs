using System;
using System.Linq;
using Camelot.DataAccess.Models;
using Camelot.Services.Abstractions;
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

            var fileModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            _autoMocker.Verify<IFilesPanelStateService, PanelModel>(m => m.GetPanelState(), Times.Once);
            _autoMocker.Verify<IDirectoryService, string>(m => m.GetAppRootDirectory(), Times.Once);

            Assert.Single(fileModel.Tabs);
            Assert.Equal(tabViewModel, fileModel.SelectedTab);
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

            var fileModel = _autoMocker.CreateInstance<FilesPanelViewModel>();

            _autoMocker.Verify<IFilesPanelStateService, PanelModel>(m => m.GetPanelState(), Times.Once);

            Assert.Equal(tabsCount, fileModel.Tabs.Count());
        }
    }
}