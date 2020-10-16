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
        private readonly IPathService _pathService;
        private readonly IArchiveProcessorFactory _archiveProcessorFactory;

        public ArchiveService(
            IPathService pathService,
            IArchiveProcessorFactory archiveProcessorFactory)
        {
            _pathService = pathService;
            _archiveProcessorFactory = archiveProcessorFactory;
        }

        public async Task PackAsync(IReadOnlyList<string> nodes, string outputFile, ArchiveType archiveType)
        {
            var archiveProcessor = _archiveProcessorFactory.Create(archiveType);

            await archiveProcessor.PackAsync(nodes, outputFile);
        }

        public async Task UnpackAsync(string archivePath, string outputDirectory)
        {
            var archiveType = GetArchiveTypeFrom(archivePath);
            var archiveProcessor = _archiveProcessorFactory.Create(archiveType);

            await archiveProcessor.UnpackAsync(archivePath, outputDirectory);
        }

        public bool CheckIfFileIsArchive(string archivePath)
        {

        }

        private ArchiveType? GetArchiveTypeFrom(string filePath)
        {
            var fileName = _pathService.GetFileNameWithoutExtension(filePath);
            var extension = _pathService.GetExtension(filePath);
            if (fileName.EndsWith(".tar"))
            {
                extension = "tar." + extension;
            }

            return extension switch
            {
                "tar" => ArchiveType.Tar,
                "zip" => ArchiveType.Zip,
                "gzip" => ArchiveType.GZip,
                "gz" => ArchiveType.Zip,
                "tar.gz" => ArchiveType.TarGz,
                "tgz" => ArchiveType.TarGz,
                "bz" => ArchiveType.TarBz,
                "tar.bz" => ArchiveType.TarBz,
                "tar.xz" => ArchiveType.TarXz,
                "xz" => ArchiveType.TarXz,
                "rar" => ArchiveType.Rar,
                "7zip" => ArchiveType.SevenZip,
                "7z" => ArchiveType.SevenZip,
                _ => null
            };
        }
    }
}