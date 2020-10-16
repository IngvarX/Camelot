using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Archive
{
    public class ArchiveService : IArchiveService
    {
        private readonly IArchiveTypeMapper _archiveTypeMapper;
        private readonly IArchiveProcessorFactory _archiveProcessorFactory;
        private readonly IPathService _pathService;

        public ArchiveService(
            IArchiveTypeMapper archiveTypeMapper,
            IArchiveProcessorFactory archiveProcessorFactory,
            IPathService pathService)
        {
            _archiveTypeMapper = archiveTypeMapper;
            _archiveProcessorFactory = archiveProcessorFactory;
            _pathService = pathService;
        }

        public async Task PackAsync(IReadOnlyList<string> nodes, string outputFile, ArchiveType archiveType)
        {
            var archiveProcessor = _archiveProcessorFactory.Create(archiveType);

            await archiveProcessor.PackAsync(nodes, outputFile);
        }

        public async Task UnpackAsync(string archivePath, string outputDirectory = null)
        {
            if (!CheckIfNodeIsArchive(archivePath))
            {
                throw new InvalidOperationException($"{archivePath} is not an archive!");
            }

            outputDirectory ??= _pathService.GetParentDirectory(archivePath);

            // ReSharper disable once PossibleInvalidOperationException
            var archiveType =_archiveTypeMapper.GetArchiveTypeFrom(archivePath).Value;
            var archiveProcessor = _archiveProcessorFactory.Create(archiveType);

            await archiveProcessor.UnpackAsync(archivePath, outputDirectory);
        }

        public bool CheckIfNodeIsArchive(string nodePath) =>
            _archiveTypeMapper.GetArchiveTypeFrom(nodePath).HasValue;
    }
}