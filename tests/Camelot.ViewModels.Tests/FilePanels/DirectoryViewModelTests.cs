using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.FilePanels;

public class DirectoryViewModelTests
{
    private const string FullPath = "/home/camelot";

    private readonly AutoMocker _autoMocker;

    public DirectoryViewModelTests()
    {
        _autoMocker = new AutoMocker();
        _autoMocker.Use(true);
    }

    [Fact]
    public void TestOpenInNewTabCommand()
    {
        var tabListMock = new Mock<ITabsListViewModel>();
        tabListMock
            .Setup(m => m.CreateNewTab(FullPath))
            .Verifiable();
        var filesPanelMock = new Mock<IFilesPanelViewModel>();
        filesPanelMock
            .SetupGet(m => m.TabsListViewModel)
            .Returns(tabListMock.Object);
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.ActiveFilesPanelViewModel)
            .Returns(filesPanelMock.Object);

        var viewModel = _autoMocker.CreateInstance<DirectoryViewModel>();
        viewModel.FullPath = FullPath;

        Assert.True(viewModel.OpenInNewTabCommand.CanExecute(null));
        viewModel.OpenInNewTabCommand.Execute(null);

        tabListMock
            .Verify(m => m.CreateNewTab(FullPath), Times.Once);
    }

    [Fact]
    public void TestOpenInNewTabOnOppositePanelCommand()
    {
        var tabListMock = new Mock<ITabsListViewModel>();
        tabListMock
            .Setup(m => m.CreateNewTab(FullPath))
            .Verifiable();
        var filesPanelMock = new Mock<IFilesPanelViewModel>();
        filesPanelMock
            .SetupGet(m => m.TabsListViewModel)
            .Returns(tabListMock.Object);
        _autoMocker
            .Setup<IFilesOperationsMediator, IFilesPanelViewModel>(m => m.InactiveFilesPanelViewModel)
            .Returns(filesPanelMock.Object);

        var viewModel = _autoMocker.CreateInstance<DirectoryViewModel>();
        viewModel.FullPath = FullPath;

        Assert.True(viewModel.OpenInNewTabOnOppositePanelCommand.CanExecute(null));
        viewModel.OpenInNewTabOnOppositePanelCommand.Execute(null);

        tabListMock
            .Verify(m => m.CreateNewTab(FullPath), Times.Once);
    }
}