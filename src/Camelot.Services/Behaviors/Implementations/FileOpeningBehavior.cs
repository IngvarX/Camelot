using Camelot.Services.Behaviors.Interfaces;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Behaviors.Implementations
{
    public class FileOpeningBehavior : IFileOpeningBehavior
    {
        private readonly IFileOpeningService _fileOpeningService;

        public FileOpeningBehavior(IFileOpeningService fileOpeningService)
        {
            _fileOpeningService = fileOpeningService;
        }

        public void Open(string file)
        {
            _fileOpeningService.Open(file);
        }
    }
}