using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Services.Implementations
{
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

        public Task CopyFilesAsync(IReadOnlyList<string> files, string fullPath) =>
            _operationsService.CopyAsync(files, ExtractDirectory(fullPath));

        public Task MoveFilesAsync(IReadOnlyList<string> files, string fullPath) =>
            _operationsService.MoveAsync(files, ExtractDirectory(fullPath));

        private string ExtractDirectory(string fullPath) => _directoryService.CheckIfExists(fullPath)
            ? fullPath
            : _pathService.GetParentDirectory(fullPath);
    }
}