using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class TabViewModelFactory : ITabViewModelFactory
    {
        private readonly IPathService _pathService;

        public TabViewModelFactory(IPathService pathService)
        {
            _pathService = pathService;
        }

        public ITabViewModel Create(TabStateModel tabModel)
        {
            var fileSystemNodeViewModel = Create(tabModel.SortingSettings);

            return new TabViewModel(_pathService, fileSystemNodeViewModel, tabModel.Directory);
        }

        private static IFileSystemNodesSortingViewModel Create(SortingSettingsStateModel sortingSettings) =>
            new FileSystemNodesSortingViewModel(sortingSettings.SortingMode, sortingSettings.IsAscending);
    }
}