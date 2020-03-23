using Camelot.Factories.Interfaces;
using Camelot.Services.Interfaces;
using Camelot.ViewModels.MainWindow;

namespace Camelot.Factories.Implementations
{
    public class TabViewModelFactory : ITabViewModelFactory
    {
        private readonly IPathService _pathService;

        public TabViewModelFactory(IPathService pathService)
        {
            _pathService = pathService;
        }

        public TabViewModel Create(string directory)
        {
            return new TabViewModel(_pathService, directory);
        }
    }
}