using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Archive
{
    public class ArchiveService : IArchiveService
    {
        private readonly IArchiveTypeMapper _archiveTypeMapper;
        private readonly IPathService _pathService;
        private readonly IOperationsService _operationsService;
        private readonly IFileNameGenerationService _fileNameGenerationService;

        public ArchiveService(
            IArchiveTypeMapper archiveTypeMapper,
            IPathService pathService,
            IOperationsService operationsService,
            IFileNameGenerationService fileNameGenerationService)
        {
            _archiveTypeMapper = archiveTypeMapper;
            _pathService = pathService;
            _operationsService = operationsService;
            _fileNameGenerationService = fileNameGenerationService;
        }

        public Task PackAsync(IReadOnlyList<string> nodes, string outputFile, ArchiveType archiveType) =>
            _operationsService.PackAsync(nodes, outputFile, archiveType);

        public async Task ExtractToNewDirectoryAsync(string archivePath)
        {
            var cleanedUpArchivePath = CleanupArchivePath(archivePath);
            var fullName = _fileNameGenerationService.GenerateFullNameWithoutExtension(cleanedUpArchivePath);

            await ExtractAsync(archivePath, fullName);
        }

        public async Task ExtractAsync(string archivePath, string outputDirectory = null)
        {
            if (!CheckIfNodeIsArchive(archivePath))
            {
                throw new InvalidOperationException($"{archivePath} is not an archive!");
            }

            outputDirectory ??= _pathService.GetParentDirectory(archivePath);
            // ReSharper disable once PossibleInvalidOperationException
            var archiveType = _archiveTypeMapper.GetArchiveTypeFrom(archivePath).Value;

            await _operationsService.ExtractAsync(archivePath, outputDirectory, archiveType);
        }

        public bool CheckIfNodeIsArchive(string nodePath) =>
            _archiveTypeMapper.GetArchiveTypeFrom(nodePath).HasValue;

        private static string CleanupArchivePath(string archivePath) =>
            archivePath.Replace(".tar", string.Empty);
    }
}