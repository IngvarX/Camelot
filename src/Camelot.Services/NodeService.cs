using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services;

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

    public NodeModelBase GetNode(string nodePath) =>
        _fileService.CheckIfExists(nodePath)
            ? _fileService.GetFile(nodePath)
            : _directoryService.GetDirectory(nodePath);
}