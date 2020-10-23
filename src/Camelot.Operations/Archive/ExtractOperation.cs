using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Operations.Archive
{
    public class ExtractOperation : OperationBase, IInternalOperation
    {
        private readonly IArchiveReader _archiveReader;
        private readonly IDirectoryService _directoryService;
        private readonly string _archiveFilePath;
        private readonly string _outputDirectory;

        public ExtractOperation(
            IArchiveReader archiveReader,
            IDirectoryService directoryService,
            string archiveFilePath,
            string outputDirectory)
        {
            _archiveReader = archiveReader;
            _directoryService = directoryService;
            _archiveFilePath = archiveFilePath;
            _outputDirectory = outputDirectory;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            CreateOutputDirectoryIfNeeded(_outputDirectory);

            State = OperationState.InProgress;
            await _archiveReader.ExtractAsync(_archiveFilePath, _outputDirectory);
            State = OperationState.Finished;
            SetFinalProgress();
        }

        private void CreateOutputDirectoryIfNeeded(string outputDirectory)
        {
            if (!_directoryService.CheckIfExists(outputDirectory))
            {
                _directoryService.Create(outputDirectory);
            }
        }
    }
}