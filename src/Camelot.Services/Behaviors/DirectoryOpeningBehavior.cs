using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;

namespace Camelot.Services.Behaviors;

public class DirectoryOpeningBehavior : IFileSystemNodeOpeningBehavior
{
    private readonly IDirectoryService _directoryService;
    private readonly IResourceOpeningService _resourceOpeningService;

    public DirectoryOpeningBehavior(
        IDirectoryService directoryService,
        IResourceOpeningService resourceOpeningService)
    {
        _directoryService = directoryService;
        _resourceOpeningService = resourceOpeningService;
    }

    public void Open(string node) => _directoryService.SelectedDirectory = node;

    public void OpenWith(string command, string arguments, string node) =>
        _resourceOpeningService.OpenWith(command, arguments, node);
}