using System.Collections.Generic;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels;

public class TabViewModelTests
{
    private const string PrevDirectory = "PrevDir";
    private const string CurrentDirectory = "CurrDir";
    private const string NextDirectory = "NextDir";
    private const string CurrentDirectoryName = "CurrDirName";

    private readonly AutoMocker _autoMocker;
    private readonly TabViewModel _tabViewModel;

    public TabViewModelTests()
    {
        _autoMocker = new AutoMocker();
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(CurrentDirectory))
            .Returns(CurrentDirectory);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetFileName(CurrentDirectory))
            .Returns(CurrentDirectoryName);
        _autoMocker
            .Use(new TabConfiguration
            {
                MaxHistorySize = 100
            });
        _autoMocker.Use(new TabStateModel
        {
            Directory = CurrentDirectory,
            History = new List<string> {PrevDirectory, PrevDirectory, CurrentDirectory, NextDirectory, NextDirectory},
            SortingSettings = new SortingSettingsStateModel(),
            CurrentPositionInHistory = 2
        });
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(It.IsAny<string>()))
            .Returns(true);

        _tabViewModel = _autoMocker.CreateInstance<TabViewModel>();
    }

    [Fact]
    public void TestProperties()
    {
        Assert.Equal(CurrentDirectory, _tabViewModel.CurrentDirectory);
        Assert.Equal(CurrentDirectoryName, _tabViewModel.DirectoryName);
        Assert.False(_tabViewModel.IsActive);
        Assert.False(_tabViewModel.IsGloballyActive);

        _tabViewModel.IsActive = _tabViewModel.IsGloballyActive = true;
        Assert.True(_tabViewModel.IsActive);
        Assert.True(_tabViewModel.IsGloballyActive);
    }

    [Fact]
    public void TestActivateCommand()
    {
        var activationRequested = false;
        _tabViewModel.ActivationRequested += (sender, args) => activationRequested = true;
        Assert.True(_tabViewModel.ActivateCommand.CanExecute(null));
        _tabViewModel.ActivateCommand.Execute(null);

        Assert.True(activationRequested);
    }

    [Fact]
    public void TestNewTabCommand()
    {
        var newTabRequested = false;
        _tabViewModel.NewTabRequested += (sender, args) => newTabRequested = true;
        Assert.True(_tabViewModel.NewTabCommand.CanExecute(null));
        _tabViewModel.NewTabCommand.Execute(null);

        Assert.True(newTabRequested);
    }

    [Fact]
    public void TestNewTabOnOppositePanelCommand()
    {
        var newTabRequested = false;
        _tabViewModel.NewTabOnOppositePanelRequested += (sender, args) => newTabRequested = true;
        Assert.True(_tabViewModel.NewTabOnOppositePanelCommand.CanExecute(null));
        _tabViewModel.NewTabOnOppositePanelCommand.Execute(null);

        Assert.True(newTabRequested);
    }

    [Fact]
    public void TestCloseCommand()
    {
        var closeRequested = false;
        _tabViewModel.CloseRequested += (sender, args) => closeRequested = true;
        Assert.True(_tabViewModel.CloseTabCommand.CanExecute(null));
        _tabViewModel.CloseTabCommand.Execute(null);

        Assert.True(closeRequested);
    }

    [Fact]
    public void TestCloseTabsToTheLeftCommand()
    {
        var closeTabsToTheLeftRequested = false;
        _tabViewModel.ClosingTabsToTheLeftRequested += (sender, args) => closeTabsToTheLeftRequested = true;
        Assert.True(_tabViewModel.CloseTabsToTheLeftCommand.CanExecute(null));
        _tabViewModel.CloseTabsToTheLeftCommand.Execute(null);

        Assert.True(closeTabsToTheLeftRequested);
    }

    [Fact]
    public void TestCloseTabsToTheRightCommand()
    {
        var closeTabsToTheRightRequested = false;
        _tabViewModel.ClosingTabsToTheRightRequested += (sender, args) => closeTabsToTheRightRequested = true;
        Assert.True(_tabViewModel.CloseTabsToTheRightCommand.CanExecute(null));
        _tabViewModel.CloseTabsToTheRightCommand.Execute(null);

        Assert.True(closeTabsToTheRightRequested);
    }

    [Fact]
    public void TestCloseAllTabsButThisCommand()
    {
        var closeAllTabsButThisRequested = false;
        _tabViewModel.ClosingAllTabsButThisRequested += (sender, args) => closeAllTabsButThisRequested = true;
        Assert.True(_tabViewModel.CloseAllTabsButThisCommand.CanExecute(null));
        _tabViewModel.CloseAllTabsButThisCommand.Execute(null);

        Assert.True(closeAllTabsButThisRequested);
    }

    [Fact]
    public void TestRequestMoveCommand()
    {
        var tab = new Mock<ITabViewModel>().Object;
        var callbackCalled = false;
        _tabViewModel.MoveRequested += (sender, args) => callbackCalled = args.Target == tab;
        Assert.True(_tabViewModel.RequestMoveCommand.CanExecute(null));
        _tabViewModel.RequestMoveCommand.Execute(tab);

        Assert.True(callbackCalled);
    }

    [Fact]
    public void TestGoToPreviousDirectoryCommand()
    {
        Assert.True(_tabViewModel.GoToPreviousDirectoryCommand.CanExecute(null));
        _tabViewModel.GoToPreviousDirectoryCommand.Execute(null);

        Assert.Equal(PrevDirectory, _tabViewModel.CurrentDirectory);
        Assert.Equal(1, _tabViewModel.GetState().CurrentPositionInHistory);

        _autoMocker
            .GetMock<IFilePanelDirectoryObserver>()
            .VerifySet(m => m.CurrentDirectory = PrevDirectory);
    }

    [Fact]
    public void TestGoToPreviousDirectoryCommandNoDirectory()
    {
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(PrevDirectory))
            .Returns(false);

        Assert.True(_tabViewModel.GoToPreviousDirectoryCommand.CanExecute(null));
        _tabViewModel.GoToPreviousDirectoryCommand.Execute(null);

        Assert.Equal(CurrentDirectory, _tabViewModel.CurrentDirectory);
        Assert.Equal(0, _tabViewModel.GetState().CurrentPositionInHistory);

        _autoMocker
            .GetMock<IFilePanelDirectoryObserver>()
            .VerifySet(m => m.CurrentDirectory = PrevDirectory,
                Times.Never);
    }

    [Fact]
    public void TestGoToNextDirectoryCommand()
    {
        Assert.True(_tabViewModel.GoToNextDirectoryCommand.CanExecute(null));
        _tabViewModel.GoToNextDirectoryCommand.Execute(null);

        Assert.Equal(NextDirectory, _tabViewModel.CurrentDirectory);
        Assert.Equal(3, _tabViewModel.GetState().CurrentPositionInHistory);

        _autoMocker
            .GetMock<IFilePanelDirectoryObserver>()
            .VerifySet(m => m.CurrentDirectory = NextDirectory);
    }

    [Fact]
    public void TestGoToNextDirectoryCommandNoDirectory()
    {
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(NextDirectory))
            .Returns(false);

        Assert.True(_tabViewModel.GoToNextDirectoryCommand.CanExecute(null));
        _tabViewModel.GoToNextDirectoryCommand.Execute(null);

        Assert.Equal(CurrentDirectory, _tabViewModel.CurrentDirectory);
        Assert.Equal(4, _tabViewModel.GetState().CurrentPositionInHistory);

        _autoMocker
            .GetMock<IFilePanelDirectoryObserver>()
            .VerifySet(m => m.CurrentDirectory = NextDirectory,
                Times.Never);
    }

    [Fact]
    public void TestSetDirectoryFails()
    {
        Assert.Equal(CurrentDirectory, _tabViewModel.CurrentDirectory);

        _tabViewModel.CurrentDirectory = CurrentDirectory;

        var state = _tabViewModel.GetState();
        Assert.Equal(5, state.History.Count);
        Assert.Equal(2, state.CurrentPositionInHistory);
    }

    [Fact]
    public void TestSetDirectorySuccess()
    {
        Assert.Equal(CurrentDirectory, _tabViewModel.CurrentDirectory);

        _tabViewModel.CurrentDirectory = PrevDirectory;

        Assert.Equal(PrevDirectory, _tabViewModel.CurrentDirectory);

        var state = _tabViewModel.GetState();
        Assert.Equal(4, state.History.Count);
        Assert.Equal(3, state.CurrentPositionInHistory);
        Assert.Equal(PrevDirectory, state.History[0]);
        Assert.Equal(PrevDirectory, state.History[1]);
        Assert.Equal(CurrentDirectory, state.History[2]);
        Assert.Equal(PrevDirectory, state.History[3]);
    }

    [Fact]
    public void TestSortingViewModelAndDirectory()
    {
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(CurrentDirectory))
            .Returns(CurrentDirectory)
            .Verifiable();
        _autoMocker
            .Setup<IPathService, string>(m => m.GetFileName(CurrentDirectory))
            .Returns(CurrentDirectory)
            .Verifiable();
        _autoMocker.Use(CurrentDirectory);

        var tabViewModel = _autoMocker.CreateInstance<TabViewModel>();

        Assert.Equal(CurrentDirectory, tabViewModel.DirectoryName);
        _autoMocker
            .Verify<IPathService>(m => m.RightTrimPathSeparators(CurrentDirectory), Times.Once);
        _autoMocker
            .Verify<IPathService>(m => m.GetFileName(CurrentDirectory), Times.Once);
    }
}