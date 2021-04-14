using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class TabViewModelFactory : ITabViewModelFactory
    {
        private readonly IPathService _pathService;
        private readonly IFilePanelDirectoryObserver _filePanelDirectoryObserver;

        public TabViewModelFactory(
            IPathService pathService,
            IFilePanelDirectoryObserver filePanelDirectoryObserver)
        {
            _pathService = pathService;
            _filePanelDirectoryObserver = filePanelDirectoryObserver;
        }

        public ITabViewModel Create(TabStateModel tabModel)
        {
            var fileSystemNodeViewModel = Create(tabModel.SortingSettings);

            return new TabViewModel(_pathService, _filePanelDirectoryObserver, fileSystemNodeViewModel, tabModel);
        }

        private static IFileSystemNodesSortingViewModel Create(SortingSettingsStateModel sortingSettings) =>
            new FileSystemNodesSortingViewModel(sortingSettings.SortingMode, sortingSettings.IsAscending);
    }
}