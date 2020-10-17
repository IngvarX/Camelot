using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Operations.Archive
{
    public class PackOperation : OperationBase, IInternalOperation
    {
        private readonly IArchiveProcessor _archiveProcessor;
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;
        private readonly IReadOnlyList<string> _nodes;
        private readonly string _outputFilePath;

        public PackOperation(
            IArchiveProcessor archiveProcessor,
            IDirectoryService directoryService,
            IPathService pathService,
            IReadOnlyList<string> nodes,
            string outputFilePath)
        {
            _archiveProcessor = archiveProcessor;
            _directoryService = directoryService;
            _pathService = pathService;
            _nodes = nodes;
            _outputFilePath = outputFilePath;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            CreateOutputDirectoryIfNeeded(_outputFilePath);

            State = OperationState.InProgress;
            await _archiveProcessor.PackAsync(_nodes, _outputFilePath);
            State = OperationState.Finished;
            SetFinalProgress();
        }

        private void CreateOutputDirectoryIfNeeded(string outputFilePath)
        {
            var outputDirectory = _pathService.GetParentDirectory(outputFilePath);
            if (!_directoryService.CheckIfExists(outputDirectory))
            {
                _directoryService.Create(outputDirectory);
            }
        }
    }
}