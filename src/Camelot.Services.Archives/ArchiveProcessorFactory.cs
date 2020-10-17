using System;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Archives.Processors;

namespace Camelot.Services.Archives
{
    public class ArchiveProcessorFactory : IArchiveProcessorFactory
    {
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;

        public ArchiveProcessorFactory(
            IFileService fileService,
            IPathService pathService)
        {
            _fileService = fileService;
            _pathService = pathService;
        }

        public IArchiveProcessor Create(ArchiveType archiveType)
        {
            switch (archiveType)
            {
                case ArchiveType.Tar:
                    return new TarArchiveProcessor(_fileService);
                case ArchiveType.Zip:
                    return new ZipArchiveProcessor(_pathService);
                case ArchiveType.TarGz:
                case ArchiveType.TarBz:
                case ArchiveType.TarXz:
                case ArchiveType.TarLz:
                case ArchiveType.GZip:
                case ArchiveType.Rar:
                case ArchiveType.SevenZip:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(archiveType), archiveType, null);
            }
        }
    }
}