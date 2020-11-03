using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Operations.Archive
{
    public class PackOperation : OperationBase, IInternalOperation
    {
        private readonly IArchiveWriter _archiveWriter;
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;
        private readonly PackOperationSettings _settings;

        public PackOperation(
            IArchiveWriter archiveWriter,
            IDirectoryService directoryService,
            IPathService pathService,
            PackOperationSettings settings)
        {
            _archiveWriter = archiveWriter;
            _directoryService = directoryService;
            _pathService = pathService;
            _settings = settings;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            CreateOutputDirectoryIfNeeded(_settings.TargetDirectory);

            State = OperationState.InProgress;
            await _archiveWriter.PackAsync(_settings.InputTopLevelFiles, _settings.InputTopLevelDirectories,
                _settings.SourceDirectory, _settings.OutputTopLevelFile);
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