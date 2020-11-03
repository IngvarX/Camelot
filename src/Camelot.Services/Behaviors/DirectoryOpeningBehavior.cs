using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;

namespace Camelot.Services.Behaviors
{
    public class DirectoryOpeningBehavior : IFileSystemNodeOpeningBehavior
    {
        private readonly IDirectoryService _directoryService;

        public DirectoryOpeningBehavior(
            IDirectoryService directoryService)
        {
            _directoryService = directoryService;
        }

        public void Open(string node) => _directoryService.SelectedDirectory = node;
    }
}