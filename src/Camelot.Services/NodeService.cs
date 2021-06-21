using Camelot.Services.Abstractions;

namespace Camelot.Services
{
    public class NodeService : INodeService
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;

        public NodeService(
            IFileService fileService,
            IDirectoryService directoryService)
        {
            _fileService = fileService;
            _directoryService = directoryService;
        }

        public bool CheckIfExists(string nodePath) =>
            _fileService.CheckIfExists(nodePath) || _directoryService.CheckIfExists(nodePath);
    }
}