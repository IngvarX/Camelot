using System;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Archives.Implementations;
using Camelot.Services.Archives.Interfaces;
using Camelot.Services.Archives.Processors;

namespace Camelot.Services.Archives
{
    public class ArchiveProcessorFactory : IArchiveProcessorFactory
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly IFileNameGenerationService _fileNameGenerationService;
        private readonly IPathService _pathService;

        public ArchiveProcessorFactory(
            IFileService fileService,
            IDirectoryService directoryService,
            IFileNameGenerationService fileNameGenerationService,
            IPathService pathService)
        {
            _fileService = fileService;
            _directoryService = directoryService;
            _fileNameGenerationService = fileNameGenerationService;
            _pathService = pathService;
        }

        public IArchiveProcessor Create(ArchiveType archiveType)
        {
            switch (archiveType)
            {
                case ArchiveType.Tar:
                    return new TarArchiveProcessor(_fileService, _directoryService);
                case ArchiveType.Zip:
                    return new ZipArchiveProcessor();
                case ArchiveType.TarGz:
                    return new TarZipArchiveProcessor(_fileService, CreateGzStreamFactory());
                case ArchiveType.GZip:
                    return new SingleFileZipArchiveProcessor(_fileService, _fileNameGenerationService, _pathService,
                        CreateGzStreamFactory());
                case ArchiveType.TarBz2:
                    return new TarZipArchiveProcessor(_fileService, CreateBz2StreamFactory());
                case ArchiveType.Bz2:
                    return new SingleFileZipArchiveProcessor(_fileService, _fileNameGenerationService, _pathService,
                        CreateBz2StreamFactory());
                case ArchiveType.TarXz:
                case ArchiveType.TarLz:
                case ArchiveType.SevenZip:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(archiveType), archiveType, null);
            }
        }

        private static IStreamFactory CreateBz2StreamFactory() => new Bzip2StreamFactory();

        private static IStreamFactory CreateGzStreamFactory() => new GzipStreamFactory();
    }
}