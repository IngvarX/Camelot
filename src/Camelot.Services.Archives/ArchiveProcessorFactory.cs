using System;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Archives
{
    public class ArchiveProcessorFactory : IArchiveProcessorFactory
    {
        private readonly IFileService _fileService;

        public ArchiveProcessorFactory(
            IFileService fileService)
        {
            _fileService = fileService;
        }

        public IArchiveProcessor Create(ArchiveType archiveType)
        {
            switch (archiveType)
            {
                case ArchiveType.Tar:
                    return new TarArchiveProcessor(_fileService);
                case ArchiveType.Zip:
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