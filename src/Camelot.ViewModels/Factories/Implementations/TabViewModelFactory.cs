using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Factories.Implementations;

public class TabViewModelFactory : ITabViewModelFactory
{
    private readonly IPathService _pathService;
    private readonly TabConfiguration _tabConfiguration;

    public TabViewModelFactory(
        IPathService pathService,
        TabConfiguration tabConfiguration)
    {
        _pathService = pathService;
        _tabConfiguration = tabConfiguration;
    }

    public ITabViewModel Create(IFilePanelDirectoryObserver observer, TabStateModel tabModel)
    {
        var fileSystemNodeViewModel = Create(tabModel.SortingSettings);

        return new TabViewModel(_pathService, observer, fileSystemNodeViewModel, _tabConfiguration, tabModel);
    }

    private static IFileSystemNodesSortingViewModel Create(SortingSettingsStateModel sortingSettings) =>
        new FileSystemNodesSortingViewModel(sortingSettings.SortingMode, sortingSettings.IsAscending);
}