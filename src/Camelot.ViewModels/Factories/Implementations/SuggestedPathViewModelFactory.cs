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

        public ISuggestedPathViewModel Create(string root, string fullPath)
        {
            if (root.Length >= fullPath.Length)
            {
                root = _pathService.GetParentDirectory(root);
            }

            var text = fullPath.Substring(root.Length);

            return new SuggestedPathViewModel(fullPath, SuggestedPathType.Directory, text);
        }
    }
}