using System.Collections.Generic;
using System.IO;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories;

public class TabViewModelFactoryTests
{
    private readonly AutoMocker _autoMocker;

    public TabViewModelFactoryTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestDirectoryName()
    {
        var directoryName = Directory.GetCurrentDirectory();
        var tabModel = new TabStateModel
        {
            Directory = directoryName,
            SortingSettings = new SortingSettingsStateModel(),
            History = new List<string> {directoryName}
        };
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(directoryName))
            .Returns(directoryName);

        var tabViewModelFactory = _autoMocker.CreateInstance<TabViewModelFactory>();
        var observer = Mock.Of<IFilePanelDirectoryObserver>();
        var tabViewModel = tabViewModelFactory.Create(observer, tabModel);

        Assert.Equal(directoryName, tabViewModel.CurrentDirectory);
    }
}