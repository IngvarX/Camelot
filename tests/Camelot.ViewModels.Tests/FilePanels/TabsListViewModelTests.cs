using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels;

public class TabsListViewModelTests
{
    private const string AppRootDirectory = "Root";
    private const string Directory = "Dir";

    private readonly AutoMocker _autoMocker;

    public TabsListViewModelTests()
    {
        _autoMocker = new AutoMocker();
        _autoMocker.Use(new TabsListConfiguration
        {
            SavedClosedTabsLimit = 5
        });
    }

    [Fact]
    public void TestEmptyFilesPanelState()
    {
        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(Mock.Of<IFileSystemNodesSortingViewModel>());
        var tabViewModel = tabViewModelMock.Object;
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.IsAny<TabStateModel>()))
            .Returns(tabViewModel);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel()
            {
                Tabs = new List<TabStateModel>()
            })
            .Verifiable();
        _autoMocker
            .Setup<IHomeDirectoryProvider, string>(m => m.HomeDirectoryPath)
            .Returns(AppRootDirectory)
            .Verifiable();
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        _autoMocker.Verify<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState(), Times.Once);
        _autoMocker.Verify<IHomeDirectoryProvider, string>(m => m.HomeDirectoryPath, Times.Once);

        Assert.Single(tabsListViewModel.Tabs);
        Assert.Equal(tabViewModel, tabsListViewModel.SelectedTab);
    }

    [Fact]
    public void TestNonEmptyFilesPanelState()
    {
        var tabsCount = new Random().Next(2, 10);

        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(Mock.Of<IFileSystemNodesSortingViewModel>());
        var tabViewModel = tabViewModelMock.Object;
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.IsAny<TabStateModel>()))
            .Returns(tabViewModel);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = Enumerable
                    .Repeat(new TabStateModel {Directory = AppRootDirectory}, tabsCount)
                    .ToList()
            })
            .Verifiable();

        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        _autoMocker.Verify<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState(), Times.Once);

        Assert.Equal(tabsCount, tabsListViewModel.Tabs.Count);
    }

    [Fact]
    public void TestReopenClosedTabNotExists()
    {
        var sortingViewModelMock = new Mock<IFileSystemNodesSortingViewModel>();
        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(sortingViewModelMock.Object);
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.IsAny<TabStateModel>()))
            .Returns(tabViewModelMock.Object);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        Assert.True(tabsListViewModel.ReopenClosedTabCommand.CanExecute(null));

        tabsListViewModel.ReopenClosedTabCommand.Execute(null);

        Assert.Single(tabsListViewModel.Tabs);
    }

    [Fact]
    public void TestReopenClosedTabExists()
    {
        var sortingViewModelMock = new Mock<IFileSystemNodesSortingViewModel>();
        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(sortingViewModelMock.Object);
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.IsAny<TabStateModel>()))
            .Returns(tabViewModelMock.Object);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        Assert.True(tabsListViewModel.CloseCurrentTabCommand.CanExecute(null));
        tabsListViewModel.CloseCurrentTabCommand.Execute(null);

        Assert.Equal(1, tabsListViewModel.Tabs.Count);

        Assert.True(tabsListViewModel.ReopenClosedTabCommand.CanExecute(null));
        tabsListViewModel.ReopenClosedTabCommand.Execute(null);

        Assert.Equal(2, tabsListViewModel.Tabs.Count);
    }

    [Fact]
    public async Task TestCreateAndCloseTab()
    {
        var sortingViewModelMock = new Mock<IFileSystemNodesSortingViewModel>();
        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(sortingViewModelMock.Object);
        tabViewModelMock
            .Setup(m => m.GetState())
            .Returns(new TabStateModel());
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.IsAny<TabStateModel>()))
            .Returns(tabViewModelMock.Object);
        _autoMocker
            .Setup<IFilesPanelStateService>(m => m.SavePanelState(It.IsAny<PanelStateModel>()))
            .Verifiable();
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();
        Assert.Single(tabsListViewModel.Tabs);

        tabsListViewModel.CreateNewTabCommand.Execute(null);
        Assert.Equal(2, tabsListViewModel.Tabs.Count);

        tabsListViewModel.CloseCurrentTabCommand.Execute(null);
        Assert.Single(tabsListViewModel.Tabs);
        tabsListViewModel.CloseCurrentTabCommand.Execute(null);
        Assert.Single(tabsListViewModel.Tabs);

        await Task.Delay(300);

        _autoMocker
            .Verify<IFilesPanelStateService>(m => m.SavePanelState(It.IsAny<PanelStateModel>()),
                Times.AtLeast(2));
    }

    [Fact]
    public async Task TestCreateTab()
    {
        var sortingViewModelMock = new Mock<IFileSystemNodesSortingViewModel>();
        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(sortingViewModelMock.Object);
        tabViewModelMock
            .Setup(m => m.GetState())
            .Returns(new TabStateModel());
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.IsAny<TabStateModel>()))
            .Returns(tabViewModelMock.Object);
        _autoMocker
            .Setup<IFilesPanelStateService>(m => m.SavePanelState(It.IsAny<PanelStateModel>()))
            .Verifiable();
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();
        Assert.Single(tabsListViewModel.Tabs);

        tabsListViewModel.CreateNewTab(Directory);
        Assert.Equal(2, tabsListViewModel.Tabs.Count);

        Assert.Equal(tabsListViewModel.Tabs[0], tabsListViewModel.SelectedTab);

        await Task.Delay(300);

        _autoMocker
            .Verify<IFilesPanelStateService>(m => m.SavePanelState(It.IsAny<PanelStateModel>()),
                Times.AtLeast(2));
    }

    [Theory]
    [InlineData(true, 0, 1)]
    [InlineData(false, 1, 0)]
    public void TestCreateOnOtherPanel(bool isActive, int activeCallsCount, int inactiveCallsCount)
    {
        var tabViewModelMock = new Mock<ITabViewModel>();
        var inactiveFilePanelMock = new Mock<IFilesPanelViewModel>();
        var activeFilePanelMock = new Mock<IFilesPanelViewModel>();
        var inactiveTabsListMock = new Mock<ITabsListViewModel>();
        var activeTabsListMock = new Mock<ITabsListViewModel>();

        tabViewModelMock
            .SetupGet(m => m.CurrentDirectory)
            .Returns(AppRootDirectory);
        tabViewModelMock
            .SetupGet(m => m.IsGloballyActive)
            .Returns(isActive);
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(Mock.Of<IFileSystemNodesSortingViewModel>());
        inactiveFilePanelMock
            .SetupGet(m => m.TabsListViewModel)
            .Returns(inactiveTabsListMock.Object);
        inactiveTabsListMock
            .Setup(m => m.CreateNewTab(AppRootDirectory))
            .Verifiable();
        activeFilePanelMock
            .SetupGet(m => m.TabsListViewModel)
            .Returns(activeTabsListMock.Object);
        activeTabsListMock
            .Setup(m => m.CreateNewTab(AppRootDirectory))
            .Verifiable();

        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.IsAny<TabStateModel>()))
            .Returns(tabViewModelMock.Object);
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.InactiveFilesPanelViewModel)
            .Returns(inactiveFilePanelMock.Object);
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
            .Returns(activeFilePanelMock.Object);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();
        Assert.Single(tabsListViewModel.Tabs);

        tabViewModelMock.Raise(m => m.NewTabOnOppositePanelRequested += null, EventArgs.Empty);
        Assert.Single(tabsListViewModel.Tabs);

        inactiveTabsListMock
            .Verify(m => m.CreateNewTab(AppRootDirectory), Times.Exactly(inactiveCallsCount));
        activeTabsListMock
            .Verify(m => m.CreateNewTab(AppRootDirectory), Times.Exactly(activeCallsCount));
    }

    [Fact]
    public void TestNoTabFound()
    {
        var sortingViewModelMock = new Mock<IFileSystemNodesSortingViewModel>();
        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(sortingViewModelMock.Object);
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(tabViewModelMock.Object);
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IHomeDirectoryProvider, string>(m => m.HomeDirectoryPath)
            .Returns(AppRootDirectory);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = Directory},
                    new TabStateModel {Directory = Directory}
                },
                SelectedTabIndex = 1
            });

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();
        Assert.Single(tabsListViewModel.Tabs);
    }

    [Fact]
    public void TestTabActivation()
    {
        var sortingViewModelMock = new Mock<IFileSystemNodesSortingViewModel>();
        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(sortingViewModelMock.Object);
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(tabViewModelMock.Object);
        var secondTabViewModelMock = new Mock<ITabViewModel>();
        secondTabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(sortingViewModelMock.Object);
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == Directory)))
            .Returns(secondTabViewModelMock.Object);
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(It.Is<string>(d => d == AppRootDirectory || d == Directory)))
            .Returns(true);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = Directory}
                }
            });

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();
        Assert.Equal(tabViewModelMock.Object, tabsListViewModel.SelectedTab);

        secondTabViewModelMock.Raise(m => m.ActivationRequested += null, EventArgs.Empty);
        Assert.Equal(secondTabViewModelMock.Object, tabsListViewModel.SelectedTab);
    }

    [Fact]
    public void TestNewTabAndCloseRequested()
    {
        var firstTabMock = Create();
        firstTabMock
            .Setup(m => m.GetState())
            .Returns(new TabStateModel()
            {
                SortingSettings = new SortingSettingsStateModel()
            });
        var currentTabMock = firstTabMock;
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(() => currentTabMock.Object);
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();
        Assert.Single(tabsListViewModel.Tabs);

        currentTabMock = Create();

        firstTabMock.Raise(m => m.NewTabRequested += null, EventArgs.Empty);
        Assert.Equal(2, tabsListViewModel.Tabs.Count);
        Assert.NotEqual(firstTabMock.Object, tabsListViewModel.SelectedTab);

        firstTabMock.Raise(m => m.CloseRequested += null, EventArgs.Empty);
        Assert.Single(tabsListViewModel.Tabs);
        Assert.Equal(currentTabMock.Object, tabsListViewModel.SelectedTab);
    }

    [Fact]
    public void TestCloseAllButThisRequested()
    {
        var tabs = new List<Mock<ITabViewModel>>();
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(() =>
            {
                var mock = Create();
                tabs.Add(mock);

                return mock.Object;
            });
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        var secondTab = tabs[1];
        secondTab.Raise(m => m.ClosingAllTabsButThisRequested += null, EventArgs.Empty);

        Assert.Single(tabsListViewModel.Tabs);
        Assert.Equal(secondTab.Object, tabsListViewModel.SelectedTab);
    }

    [Fact]
    public void TestCloseAllToTheLeftRequested()
    {
        var tabs = new List<Mock<ITabViewModel>>();
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(() =>
            {
                var mock = Create();
                tabs.Add(mock);

                return mock.Object;
            });
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        var secondTab = tabs[1];
        secondTab.Raise(m => m.ClosingTabsToTheLeftRequested += null, EventArgs.Empty);

        Assert.Equal(2, tabsListViewModel.Tabs.Count);
        Assert.Equal(secondTab.Object, tabsListViewModel.SelectedTab);
        Assert.Equal(tabs[2].Object, tabsListViewModel.Tabs.Last());
    }

    [Fact]
    public void TestCloseAllToTheRightRequested()
    {
        var tabs = new List<Mock<ITabViewModel>>();
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(() =>
            {
                var mock = Create();
                tabs.Add(mock);

                return mock.Object;
            });
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        var secondTab = tabs[1];
        secondTab.Raise(m => m.ClosingTabsToTheRightRequested += null, EventArgs.Empty);

        Assert.Equal(2, tabsListViewModel.Tabs.Count);
        Assert.Equal(tabs[0].Object, tabsListViewModel.SelectedTab);
        Assert.Equal(secondTab.Object, tabsListViewModel.Tabs.Last());
    }

    [Fact]
    public void TestSelectTabsCommands()
    {
        var tabs = new List<Mock<ITabViewModel>>();
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(() =>
            {
                var mock = Create();
                tabs.Add(mock);

                return mock.Object;
            });
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        Assert.Equal(tabs[0].Object, tabsListViewModel.SelectedTab);

        for (var i = 0; i < 10; i++)
        {
            Assert.True(tabsListViewModel.SelectTabToTheRightCommand.CanExecute(null));
            tabsListViewModel.SelectTabToTheRightCommand.Execute(null);

            var index = Math.Min(tabs.Count - 1, i + 1);
            Assert.Equal(tabs[index].Object, tabsListViewModel.SelectedTab);
        }

        for (var i = 0; i < 10; i++)
        {
            Assert.True(tabsListViewModel.SelectTabToTheLeftCommand.CanExecute(null));
            tabsListViewModel.SelectTabToTheLeftCommand.Execute(null);

            var index = Math.Max(tabs.Count - i - 2, 0);
            Assert.Equal(tabs[index].Object, tabsListViewModel.SelectedTab);
        }
    }

    [Fact]
    public void TestSelectValidTab()
    {
        var tabsCount = new Random().Next(2, 10);

        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(Mock.Of<IFileSystemNodesSortingViewModel>());
        var tabViewModel = tabViewModelMock.Object;
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.IsAny<TabStateModel>()))
            .Returns(tabViewModel);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = Enumerable
                    .Repeat(new TabStateModel {Directory = AppRootDirectory}, tabsCount)
                    .ToList()
            })
            .Verifiable();

        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        Assert.Equal(tabsListViewModel.Tabs[0], tabsListViewModel.SelectedTab);

        for (var i = 0; i < tabsCount; i++)
        {
            tabsListViewModel.GoToTabCommand.Execute(i);
            Assert.Equal(tabsListViewModel.Tabs[i], tabsListViewModel.SelectedTab);
        }
    }

    [Fact]
    public void TestSelectInvalidTab()
    {
        var tabsCount = new Random().Next(2, 10);

        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(Mock.Of<IFileSystemNodesSortingViewModel>());
        var tabViewModel = tabViewModelMock.Object;
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.IsAny<TabStateModel>()))
            .Returns(tabViewModel);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = Enumerable
                    .Repeat(new TabStateModel {Directory = AppRootDirectory}, tabsCount)
                    .ToList()
            })
            .Verifiable();

        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        var tab = tabsListViewModel.Tabs[0];
        Assert.Equal(tab, tabsListViewModel.SelectedTab);

        tabsListViewModel.GoToTabCommand.Execute(-1);
        Assert.Equal(tab, tabsListViewModel.SelectedTab);

        tabsListViewModel.GoToTabCommand.Execute(tabsCount + 13);
        Assert.Equal(tab, tabsListViewModel.SelectedTab);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void TestGoToLastTabCommand(int tabsCount)
    {
        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(Mock.Of<IFileSystemNodesSortingViewModel>());
        var tabViewModel = tabViewModelMock.Object;
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.IsAny<TabStateModel>()))
            .Returns(tabViewModel);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = Enumerable
                    .Repeat(new TabStateModel {Directory = AppRootDirectory}, tabsCount)
                    .ToList()
            })
            .Verifiable();

        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        Assert.Equal(tabsListViewModel.Tabs[^1], tabsListViewModel.SelectedTab);

        tabsListViewModel.GoToTabCommand.Execute(0);

        tabsListViewModel.GoToLastTabCommand.Execute(null);
        Assert.Equal(tabsListViewModel.Tabs[^1], tabsListViewModel.SelectedTab);
    }

    [Fact]
    public void TestMoveRequestedMoveLeft()
    {
        var tabs = new List<Mock<ITabViewModel>>();
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(() =>
            {
                var mock = Create();
                tabs.Add(mock);

                return mock.Object;
            });
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        var firstTab = tabs[0].Object;
        var secondTabMock = tabs[1];
        var thirdTab = tabs[2].Object;
        var args = new TabMoveRequestedEventArgs(firstTab);
        secondTabMock.Raise(m => m.MoveRequested += null, args);

        Assert.Equal(3, tabsListViewModel.Tabs.Count);
        Assert.Equal(secondTabMock.Object, tabsListViewModel.Tabs[0]);
        Assert.Equal(firstTab, tabsListViewModel.Tabs[1]);
        Assert.Equal(thirdTab, tabsListViewModel.Tabs[2]);
    }

    [Fact]
    public void TestMoveRequestedMoveRight()
    {
        var tabs = new List<Mock<ITabViewModel>>();
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(() =>
            {
                var mock = Create();
                tabs.Add(mock);

                return mock.Object;
            });
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        var firstTab = tabs[0].Object;
        var secondTabMock = tabs[1];
        var thirdTab = tabs[2].Object;
        var args = new TabMoveRequestedEventArgs(thirdTab);
        secondTabMock.Raise(m => m.MoveRequested += null, args);

        Assert.Equal(3, tabsListViewModel.Tabs.Count);
        Assert.Equal(secondTabMock.Object, tabsListViewModel.Tabs[2]);
        Assert.Equal(firstTab, tabsListViewModel.Tabs[0]);
        Assert.Equal(thirdTab, tabsListViewModel.Tabs[1]);
    }

    [Fact]
    public void TestMoveRequestedSingleTab()
    {
        var tabs = new List<Mock<ITabViewModel>>();
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(() =>
            {
                var mock = Create();
                tabs.Add(mock);

                return mock.Object;
            });
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        var firstTab = tabs[0].Object;
        var secondTabMock = new Mock<ITabViewModel>();
        var args = new TabMoveRequestedEventArgs(secondTabMock.Object);
        tabs[0].Raise(m => m.MoveRequested += null, args);

        Assert.Single(tabsListViewModel.Tabs);
        Assert.Equal(firstTab, tabsListViewModel.Tabs.Single());
    }

    [Theory]
    [InlineData(false, 1, 0)]
    [InlineData(true, 0, 1)]
    public void TestMoveRequestedOppositePanel(bool isGloballyActive, int activePanelCallsCount,
        int inactivePanelCallsCount)
    {
        var tabs = new List<Mock<ITabViewModel>>();
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(() =>
            {
                var mock = Create();
                tabs.Add(mock);

                return mock.Object;
            });
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        var secondTabMock = tabs[1];
        tabs[0]
            .SetupGet(m => m.IsGloballyActive)
            .Returns(isGloballyActive);
        var tabFromOppositePanel = Mock.Of<ITabViewModel>();
        var args = new TabMoveRequestedEventArgs(tabFromOppositePanel);

        var inactiveTabsListMock = new Mock<ITabsListViewModel>();
        inactiveTabsListMock
            .Setup(m => m.InsertBeforeTab(tabFromOppositePanel, secondTabMock.Object))
            .Verifiable();
        var inactivePanelMock = new Mock<IFilesPanelViewModel>();
        inactivePanelMock
            .Setup(m => m.TabsListViewModel)
            .Returns(inactiveTabsListMock.Object);
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.InactiveFilesPanelViewModel)
            .Returns(inactivePanelMock.Object);

        var activeTabsListMock = new Mock<ITabsListViewModel>();
        activeTabsListMock
            .Setup(m => m.InsertBeforeTab(tabFromOppositePanel, secondTabMock.Object))
            .Verifiable();
        var activePanelMock = new Mock<IFilesPanelViewModel>();
        activePanelMock
            .Setup(m => m.TabsListViewModel)
            .Returns(activeTabsListMock.Object);
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
            .Returns(activePanelMock.Object);

        secondTabMock.Raise(m => m.MoveRequested += null, args);

        Assert.Equal(2, tabsListViewModel.Tabs.Count);

        inactiveTabsListMock
            .Verify(m => m.InsertBeforeTab(tabFromOppositePanel, secondTabMock.Object),
                Times.Exactly(inactivePanelCallsCount));
        activeTabsListMock
            .Verify(m => m.InsertBeforeTab(tabFromOppositePanel, secondTabMock.Object),
                Times.Exactly(activePanelCallsCount));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TestInsertBeforeTab(int insertIndex)
    {
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(() =>
            {
                var mock = Create();

                return mock.Object;
            });
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory},
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();

        var insertViewModelMock = new Mock<ITabViewModel>();
        insertViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(Mock.Of<IFileSystemNodesSortingViewModel>());
        var insertViewModel = insertViewModelMock.Object;
        var currentTabViewModel = tabsListViewModel.Tabs[insertIndex];
        tabsListViewModel.InsertBeforeTab(currentTabViewModel, insertViewModel);

        Assert.Equal(insertViewModel, tabsListViewModel.Tabs[insertIndex]);
    }

    [Fact]
    public async Task TestSortingSettingsChanged()
    {
        var sortingViewModelMock = new Mock<IFileSystemNodesSortingViewModel>();
        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(sortingViewModelMock.Object);
        _autoMocker
            .Setup<ITabViewModelFactory, ITabViewModel>(m => m.Create(It.IsAny<IFilePanelDirectoryObserver>(), It.Is<TabStateModel>(tm => tm.Directory == AppRootDirectory)))
            .Returns(tabViewModelMock.Object);
        _autoMocker
            .Setup<IFilesPanelStateService>(m => m.SavePanelState(It.IsAny<PanelStateModel>()))
            .Verifiable();
        _autoMocker
            .Setup<IFilesPanelStateService, PanelStateModel>(m => m.GetPanelState())
            .Returns(new PanelStateModel
            {
                Tabs = new List<TabStateModel>
                {
                    new TabStateModel {Directory = AppRootDirectory}
                }
            });

        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(AppRootDirectory))
            .Returns(true);

        var tabsListViewModel = _autoMocker.CreateInstance<TabsListViewModel>();
        Assert.NotNull(tabsListViewModel.SelectedTab);

        await Task.Delay(300);

        _autoMocker
            .Verify<IFilesPanelStateService>(m => m.SavePanelState(It.IsAny<PanelStateModel>()),
                Times.Once);

        sortingViewModelMock.Raise(m => m.SortingSettingsChanged += null, EventArgs.Empty);
        await Task.Delay(300);

        _autoMocker
            .Verify<IFilesPanelStateService>(m => m.SavePanelState(It.IsAny<PanelStateModel>()),
                Times.Exactly(2));
    }

    private static Mock<ITabViewModel> Create()
    {
        var sortingViewModelMock = new Mock<IFileSystemNodesSortingViewModel>();
        var tabViewModelMock = new Mock<ITabViewModel>();
        tabViewModelMock
            .SetupGet(m => m.SortingViewModel)
            .Returns(sortingViewModelMock.Object);
        tabViewModelMock
            .SetupGet(m => m.CurrentDirectory)
            .Returns(AppRootDirectory);

        return tabViewModelMock;
    }
}