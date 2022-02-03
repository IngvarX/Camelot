using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Camelot.ViewModels.Implementations;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.Operations;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using Camelot.ViewModels.Interfaces.Menu;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests;

public class MainWindowViewModelTests
{
    private readonly AutoMocker _autoMocker;

    public MainWindowViewModelTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestFilePanelsRegistrations()
    {
        var leftFilePanelViewModelMock = new Mock<IFilesPanelViewModel>();
        var tabsListViewModelMock = new Mock<ITabsListViewModel>();
        leftFilePanelViewModelMock
            .Setup(m => m.TabsListViewModel)
            .Returns(tabsListViewModelMock.Object);
        var rightFilePanelViewModelMock = new Mock<IFilesPanelViewModel>();
        var operationsViewModelMock = new Mock<IOperationsViewModel>();
        var menuViewModelMock = new Mock<IMenuViewModel>();
        var operationsStateViewModelMock = new Mock<IOperationsStateViewModel>();
        var topOperationsViewModelMock = new Mock<ITopOperationsViewModel>();
        var drivesListViewModelMock = new Mock<IDrivesListViewModel>();
        var fileOperationsMediatorMock = new Mock<IFilesOperationsMediator>();
        fileOperationsMediatorMock
            .Setup(m => m.Register(It.IsAny<IFilesPanelViewModel>(), It.IsAny<IFilesPanelViewModel>()))
            .Verifiable();
        fileOperationsMediatorMock
            .Setup(m => m.ActiveFilesPanelViewModel)
            .Returns(leftFilePanelViewModelMock.Object);
        var favouriteDirectoriesListViewModelMock = new Mock<IFavouriteDirectoriesListViewModel>();

        var mainWindowViewModel = new MainWindowViewModel(
            fileOperationsMediatorMock.Object,
            operationsViewModelMock.Object,
            leftFilePanelViewModelMock.Object,
            rightFilePanelViewModelMock.Object,
            menuViewModelMock.Object,
            operationsStateViewModelMock.Object,
            topOperationsViewModelMock.Object,
            drivesListViewModelMock.Object,
            favouriteDirectoriesListViewModelMock.Object);

        Assert.Equal(tabsListViewModelMock.Object, mainWindowViewModel.ActiveTabsListViewModel);
        Assert.Equal(leftFilePanelViewModelMock.Object, mainWindowViewModel.LeftFilesPanelViewModel);
        Assert.Equal(rightFilePanelViewModelMock.Object, mainWindowViewModel.RightFilesPanelViewModel);
        Assert.Equal(menuViewModelMock.Object, mainWindowViewModel.MenuViewModel);
        Assert.Equal(operationsViewModelMock.Object, mainWindowViewModel.OperationsViewModel);
        Assert.Equal(operationsStateViewModelMock.Object, mainWindowViewModel.OperationsStateViewModel);
        Assert.Equal(topOperationsViewModelMock.Object, mainWindowViewModel.TopOperationsViewModel);
        Assert.Equal(drivesListViewModelMock.Object, mainWindowViewModel.DrivesListViewModel);
        Assert.Equal(favouriteDirectoriesListViewModelMock.Object,
            mainWindowViewModel.FavouriteDirectoriesListViewModel);

        fileOperationsMediatorMock
            .Verify(m => m.Register(It.IsAny<IFilesPanelViewModel>(), It.IsAny<IFilesPanelViewModel>()),
                Times.Once);
    }

    [Fact]
    public void TestSearchCommand()
    {
        _autoMocker
            .Setup<IFilesOperationsMediator>(m => m.ToggleSearchPanelVisibility())
            .Verifiable();

        var mainWindowViewModel = _autoMocker.CreateInstance<MainWindowViewModel>();

        Assert.True(mainWindowViewModel.SearchCommand.CanExecute(null));
        mainWindowViewModel.SearchCommand.Execute(null);

        _autoMocker
            .Verify<IFilesOperationsMediator>(m => m.ToggleSearchPanelVisibility(), Times.Once);
    }

    [Fact]
    public void TestSwitchPanelCommand()
    {
        var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.InactiveFilesPanelViewModel)
            .Returns(filePanelViewModelMock.Object);

        var mainWindowViewModel = _autoMocker.CreateInstance<MainWindowViewModel>();

        Assert.True(mainWindowViewModel.SwitchPanelCommand.CanExecute(null));
        mainWindowViewModel.SwitchPanelCommand.Execute(null);

        filePanelViewModelMock.Verify(m => m.Activate(), Times.Once);
    }

    [Fact]
    public void TestFocusDirectorySelectorCommand()
    {
        var directorySelectorMock = new Mock<IDirectorySelectorViewModel>();
        var filePanelViewModelMock = new Mock<IFilesPanelViewModel>();
        filePanelViewModelMock
            .Setup(m => m.DirectorySelectorViewModel)
            .Returns(directorySelectorMock.Object);
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
            .Returns(filePanelViewModelMock.Object);

        var mainWindowViewModel = _autoMocker.CreateInstance<MainWindowViewModel>();

        Assert.True(mainWindowViewModel.FocusDirectorySelectorCommand.CanExecute(null));
        mainWindowViewModel.FocusDirectorySelectorCommand.Execute(null);

        directorySelectorMock.Verify(m => m.Activate(), Times.Once);
    }

    [Fact]
    public void TestActivePanelChanged()
    {
        var activeDirectorySelectorMock = new Mock<IDirectorySelectorViewModel>();
        var activeTabListViewModelMock = new Mock<ITabsListViewModel>();
        var activeFilePanelViewModelMock = new Mock<IFilesPanelViewModel>();
        activeFilePanelViewModelMock
            .Setup(m => m.DirectorySelectorViewModel)
            .Returns(activeDirectorySelectorMock.Object);
        activeFilePanelViewModelMock
            .Setup(m => m.TabsListViewModel)
            .Returns(activeTabListViewModelMock.Object);
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
            .Returns(activeFilePanelViewModelMock.Object);
        var inactiveDirectorySelectorMock = new Mock<IDirectorySelectorViewModel>();
        var inactiveTabListViewModelMock = new Mock<ITabsListViewModel>();
        var inactiveFilePanelViewModelMock = new Mock<IFilesPanelViewModel>();
        inactiveFilePanelViewModelMock
            .Setup(m => m.DirectorySelectorViewModel)
            .Returns(inactiveDirectorySelectorMock.Object);
        inactiveFilePanelViewModelMock
            .Setup(m => m.TabsListViewModel)
            .Returns(inactiveTabListViewModelMock.Object);
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.InactiveFilesPanelViewModel)
            .Returns(activeFilePanelViewModelMock.Object);

        var mainWindowViewModel = _autoMocker.CreateInstance<MainWindowViewModel>();

        Assert.Equal(activeDirectorySelectorMock.Object, mainWindowViewModel.ActiveDirectorySelectorViewModel);
        Assert.Equal(activeTabListViewModelMock.Object, mainWindowViewModel.ActiveTabsListViewModel);

        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
            .Returns(inactiveFilePanelViewModelMock.Object);
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.InactiveFilesPanelViewModel)
            .Returns(activeFilePanelViewModelMock.Object);
        var notifyPropertyChanged = (INotifyPropertyChanged) mainWindowViewModel;
        var properties = new List<string>();
        notifyPropertyChanged.PropertyChanged += (sender, args) => properties.Add(args.PropertyName);

        _autoMocker
            .GetMock<IFilesOperationsMediator>()
            .Raise(m => m.ActiveFilesPanelChanged += null, EventArgs.Empty);

        Assert.Equal(inactiveDirectorySelectorMock.Object, mainWindowViewModel.ActiveDirectorySelectorViewModel);
        Assert.Equal(inactiveTabListViewModelMock.Object, mainWindowViewModel.ActiveTabsListViewModel);

        var expectedProperties = new[]
        {
            nameof(mainWindowViewModel.ActiveDirectorySelectorViewModel),
            nameof(mainWindowViewModel.ActiveTabsListViewModel)
        };
        Assert.True(expectedProperties.All(p => properties.Contains(p)));
    }
}