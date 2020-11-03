using System;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Archives.Implementations;
using Camelot.Services.Archives.Interfaces;
using SharpCompress.Common;
using SharpCompress.Writers;
using ArchiveType = Camelot.Services.Abstractions.Models.Enums.ArchiveType;
using InternalArchiveType = SharpCompress.Common.ArchiveType;

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

        public IArchiveReader CreateReader(ArchiveType archiveType)
        {
            switch (archiveType)
            {
                case ArchiveType.Zip:
                case ArchiveType.Tar:
                case ArchiveType.TarGz:
                case ArchiveType.TarBz2:
                case ArchiveType.TarXz:
                case ArchiveType.TarLz:
                case ArchiveType.Gz:
                case ArchiveType.SevenZip:
                    return CreateDefaultArchiveReader();
                case ArchiveType.Xz:
                    return SingleFileZipArchiveReader(new XzStreamFactory());
                case ArchiveType.Lz:
                    return SingleFileZipArchiveReader(new LzipStreamFactory());
                case ArchiveType.Bz2:
                    return SingleFileZipArchiveReader(new Bz2StreamFactory());
                default:
                    throw new ArgumentOutOfRangeException(nameof(archiveType), archiveType, null);
            }
        }

        public IArchiveWriter CreateWriter(ArchiveType archiveType) =>
            archiveType switch
            {
                ArchiveType.Tar => CreateArchiveWriter(InternalArchiveType.Tar, CompressionType.None),
                ArchiveType.Zip => CreateArchiveWriter(InternalArchiveType.Zip, CompressionType.Deflate),
                ArchiveType.TarGz => CreateArchiveWriter(InternalArchiveType.Tar, CompressionType.GZip),
                ArchiveType.Gz => CreateArchiveWriter(InternalArchiveType.GZip, CompressionType.GZip),
                ArchiveType.TarBz2 => CreateArchiveWriter(InternalArchiveType.Tar, CompressionType.BZip2),
                ArchiveType.TarXz => CreateArchiveWriter(InternalArchiveType.Tar, CompressionType.Xz),
                ArchiveType.TarLz => CreateArchiveWriter(InternalArchiveType.Tar, CompressionType.LZip),
                _ => throw new ArgumentOutOfRangeException(nameof(archiveType), archiveType, null)
            };

        private IArchiveReader CreateDefaultArchiveReader() => new ArchiveReader(_fileService);

        private IArchiveReader SingleFileZipArchiveReader(IStreamFactory streamFactory) =>
            new SingleFileZipArchiveReader(_fileService, _fileNameGenerationService, _pathService, streamFactory);

        private IArchiveWriter CreateArchiveWriter(InternalArchiveType archiveType, WriterOptions options) =>
            new ArchiveWriter(_fileService, _pathService, _directoryService, archiveType, options);
    }
}