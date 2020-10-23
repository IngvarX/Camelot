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

        public IArchiveProcessor Create(ArchiveType archiveType) =>
            archiveType switch
            {
                ArchiveType.Tar => new TarArchiveProcessor(_fileService, _directoryService),
                ArchiveType.Zip => new ZipArchiveProcessor(),
                ArchiveType.TarGz => CreateTarZipArchiveProcessor(CreateGzStreamFactory()),
                ArchiveType.GZip => CreateSingleFileZipArchiveProcessor(CreateGzStreamFactory()),
                ArchiveType.TarBz2 =>CreateTarZipArchiveProcessor(CreateBz2StreamFactory()),
                ArchiveType.Bz2 => CreateSingleFileZipArchiveProcessor(CreateBz2StreamFactory()),
                ArchiveType.TarXz => CreateTarZipArchiveProcessor(CreateXzStreamFactory()),
                ArchiveType.Xz => CreateSingleFileZipArchiveProcessor(CreateXzStreamFactory()),
                ArchiveType.TarLz => CreateTarZipArchiveProcessor(CreateLzStreamFactory()),
                ArchiveType.Lz => CreateSingleFileZipArchiveProcessor(CreateLzStreamFactory()),
                ArchiveType.SevenZip => null,
                _ => throw new ArgumentOutOfRangeException(nameof(archiveType), archiveType, null)
            };

        private IArchiveProcessor CreateSingleFileZipArchiveProcessor(IStreamFactory factory) =>
            new SingleFileZipArchiveProcessor(_fileService, _fileNameGenerationService,
                _pathService, factory);

        private IArchiveProcessor CreateTarZipArchiveProcessor(IStreamFactory factory) =>
            new TarZipArchiveProcessor(_fileService, factory);

        private static IStreamFactory CreateBz2StreamFactory() => new Bzip2StreamFactory();

        private static IStreamFactory CreateGzStreamFactory() => new GzipStreamFactory();

        private static IStreamFactory CreateXzStreamFactory() => new XzStreamFactory();

        private static IStreamFactory CreateLzStreamFactory() => new LzStreamFactory();
    }
}