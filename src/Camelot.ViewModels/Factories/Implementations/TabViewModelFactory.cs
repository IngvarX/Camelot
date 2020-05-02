using Camelot.DataAccess.Models;
using Camelot.Services.Abstractions;
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

        public ITabViewModel Create(TabModel tabModel)
        {
            var fileSystemNodeViewModel = Create(tabModel.SortingSettings);
            
            return new TabViewModel(_pathService, fileSystemNodeViewModel, tabModel.Directory);
        }

        private static IFileSystemNodesSortingViewModel Create(SortingSettings sortingSettings)
        {
            var sortingColumn = (SortingColumn) sortingSettings.SortingMode;
            
            return new FileSystemNodesSortingViewModel(sortingColumn, sortingSettings.IsAscending);
        }
    }
}