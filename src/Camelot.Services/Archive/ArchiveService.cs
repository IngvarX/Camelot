using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Archive
{
    public class ArchiveService : IArchiveService
    {
        private readonly IArchiveTypeMapper _archiveTypeMapper;
        private readonly IArchiveProcessorFactory _archiveProcessorFactory;

        public ArchiveService(
            IArchiveTypeMapper archiveTypeMapper,
            IArchiveProcessorFactory archiveProcessorFactory)
        {
            _archiveTypeMapper = archiveTypeMapper;
            _archiveProcessorFactory = archiveProcessorFactory;
        }

        public async Task PackAsync(IReadOnlyList<string> nodes, string outputFile, ArchiveType archiveType)
        {
            var archiveProcessor = _archiveProcessorFactory.Create(archiveType);

            await archiveProcessor.PackAsync(nodes, outputFile);
        }

        public async Task UnpackAsync(string archivePath, string outputDirectory)
        {
            if (!CheckIfFileIsArchive(archivePath))
            {
                throw new InvalidOperationException($"{archivePath} is not an archive!");
            }

            // ReSharper disable once PossibleInvalidOperationException
            var archiveType =_archiveTypeMapper.GetArchiveTypeFrom(archivePath).Value;
            var archiveProcessor = _archiveProcessorFactory.Create(archiveType);

            await archiveProcessor.UnpackAsync(archivePath, outputDirectory);
        }

        public bool CheckIfFileIsArchive(string archivePath) =>
            _archiveTypeMapper.GetArchiveTypeFrom(archivePath).HasValue;
        //
        // private ArchiveType? GetArchiveTypeFrom(string filePath)
        // {
        //     var fileName = _pathService.GetFileNameWithoutExtension(filePath);
        //     var extension = _pathService.GetExtension(filePath);
        //     if (fileName.EndsWith(".tar"))
        //     {
        //         extension = "tar." + extension;
        //     }
        //
        //     return extension switch
        //     {
        //         "tar" => ArchiveType.Tar,
        //         "zip" => ArchiveType.Zip,
        //         "gzip" => ArchiveType.GZip,
        //         "gz" => ArchiveType.Zip,
        //         "tar.gz" => ArchiveType.TarGz,
        //         "tgz" => ArchiveType.TarGz,
        //         "bz" => ArchiveType.TarBz,
        //         "tar.bz" => ArchiveType.TarBz,
        //         "tar.xz" => ArchiveType.TarXz,
        //         "xz" => ArchiveType.TarXz,
        //         "rar" => ArchiveType.Rar,
        //         "7zip" => ArchiveType.SevenZip,
        //         "7z" => ArchiveType.SevenZip,
        //         _ => null
        //     };
        // }
    }
}