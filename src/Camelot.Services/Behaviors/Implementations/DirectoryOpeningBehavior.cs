using Camelot.Services.Behaviors.Interfaces;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Behaviors.Implementations
{
    public class DirectoryOpeningBehavior : IFileSystemNodeOpeningBehavior
    {
        private readonly IDirectoryService _directoryService;

        public DirectoryOpeningBehavior(
            IDirectoryService directoryService)
        {
            _directoryService = directoryService;
        }

        public void Open(string directory) => _directoryService.SelectedDirectory = directory;
    }
}