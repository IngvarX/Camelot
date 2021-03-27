using Camelot.Services.Abstractions;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class SuggestedPathViewModelFactory : ISuggestedPathViewModelFactory
    {
        private readonly IPathService _pathService;

        public SuggestedPathViewModelFactory(
            IPathService pathService)
        {
            _pathService = pathService;
        }

        public ISuggestedPathViewModel Create(string searchText, string fullPath)
        {
            var root = _pathService.GetParentDirectory(searchText);
            var relativePath = _pathService.GetRelativePath(root, fullPath);
            var text = _pathService.LeftTrimPathSeparators(relativePath);

            return new SuggestedPathViewModel(fullPath, SuggestedPathType.Directory, text);
        }
    }
}