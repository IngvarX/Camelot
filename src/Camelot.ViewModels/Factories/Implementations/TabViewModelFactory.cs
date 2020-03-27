using Camelot.Services.Interfaces;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class TabViewModelFactory : ITabViewModelFactory
    {
        private readonly IPathService _pathService;

        public TabViewModelFactory(IPathService pathService)
        {
            _pathService = pathService;
        }

        public ITabViewModel Create(string directory) => new TabViewModel(_pathService, directory);
    }
}