using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;

namespace Camelot.Services.Behaviors
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