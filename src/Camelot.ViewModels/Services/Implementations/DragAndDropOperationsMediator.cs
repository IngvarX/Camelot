using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Services.Implementations;

public class DragAndDropOperationsMediator : IDragAndDropOperationsMediator
{
    private readonly IOperationsService _operationsService;
    private readonly IDirectoryService _directoryService;
    private readonly IPathService _pathService;

    public DragAndDropOperationsMediator(
        IOperationsService operationsService,
        IDirectoryService directoryService,
        IPathService pathService)
    {
        _operationsService = operationsService;
        _directoryService = directoryService;
        _pathService = pathService;
    }

    public async Task CopyFilesAsync(IReadOnlyList<string> files, string fullPath)
    {
        var targetDirectory = ExtractDirectory(fullPath);
        var filteredFiles = Filter(files, targetDirectory);
        if (filteredFiles.Any())
        {
            await _operationsService.CopyAsync(filteredFiles, targetDirectory);
        }
    }

    public async Task MoveFilesAsync(IReadOnlyList<string> files, string fullPath)
    {
        var targetDirectory = ExtractDirectory(fullPath);
        var filteredFiles = Filter(files, targetDirectory);
        if (filteredFiles.Any())
        {
            await _operationsService.MoveAsync(filteredFiles, targetDirectory);
        }
    }

    private string ExtractDirectory(string fullPath) => _directoryService.CheckIfExists(fullPath)
        ? fullPath
        : _pathService.GetParentDirectory(fullPath);

    private IReadOnlyList<string> Filter(IReadOnlyList<string> files, string targetDirectory) =>
        files.Where(f => _pathService.GetParentDirectory(f) != targetDirectory).ToArray();
}