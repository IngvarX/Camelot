using Camelot.Services.Behaviors.Interfaces;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Behaviors.Implementations
{
    public class DirectoryOpeningBehavior : IFileOpeningBehavior
    {
        private readonly IDirectoryService _directoryService;
        private readonly IFilesSelectionService _filesSelectionService;

        public DirectoryOpeningBehavior(
            IDirectoryService directoryService,
            IFilesSelectionService filesSelectionService)
        {
            _directoryService = directoryService;
            _filesSelectionService = filesSelectionService;
        }

        public void Open(string directory)
        {
            _directoryService.SelectedDirectory = directory;
            _filesSelectionService.ClearSelectedFiles();
        }
    }
}