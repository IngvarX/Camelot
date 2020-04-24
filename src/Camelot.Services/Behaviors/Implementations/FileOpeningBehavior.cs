using Camelot.Services.Behaviors.Interfaces;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Behaviors.Implementations
{
    public class FileOpeningBehavior : IFileSystemNodeOpeningBehavior
    {
        private readonly IResourceOpeningService _resourceOpeningService;

        public FileOpeningBehavior(IResourceOpeningService resourceOpeningService)
        {
            _resourceOpeningService = resourceOpeningService;
        }

        public void Open(string file) => _resourceOpeningService.Open(file);
    }
}