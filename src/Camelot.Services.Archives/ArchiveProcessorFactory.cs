using System;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using SharpCompress.Common;
using SharpCompress.Writers;
using ArchiveType = Camelot.Services.Abstractions.Models.Enums.ArchiveType;
using InternalArchiveType = SharpCompress.Common.ArchiveType;

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

        public IArchiveReader CreateReader(ArchiveType archiveType) =>
            new ArchiveReader(_fileService);

        public IArchiveWriter CreateWriter(ArchiveType archiveType) =>
            archiveType switch
            {
                ArchiveType.Tar => CreateArchiveWriter(InternalArchiveType.Tar, CompressionType.None),
                ArchiveType.Zip => CreateArchiveWriter(InternalArchiveType.Zip, CompressionType.Deflate),
                ArchiveType.TarGz => CreateArchiveWriter(InternalArchiveType.Tar, CompressionType.GZip),
                ArchiveType.GZip => CreateArchiveWriter(InternalArchiveType.Zip, CompressionType.GZip),
                ArchiveType.TarBz2 => CreateArchiveWriter(InternalArchiveType.Tar, CompressionType.BZip2),
                ArchiveType.Bz2 => CreateArchiveWriter(InternalArchiveType.Zip, CompressionType.BZip2),
                ArchiveType.TarXz => CreateArchiveWriter(InternalArchiveType.Tar, CompressionType.Xz),
                ArchiveType.Xz => CreateArchiveWriter(InternalArchiveType.Zip, CompressionType.Xz),
                ArchiveType.TarLz => CreateArchiveWriter(InternalArchiveType.Tar, CompressionType.LZip),
                ArchiveType.Lz => CreateArchiveWriter(InternalArchiveType.Zip, CompressionType.LZip),
                ArchiveType.SevenZip => CreateArchiveWriter(InternalArchiveType.SevenZip, CompressionType.LZMA),
                _ => throw new ArgumentOutOfRangeException(nameof(archiveType), archiveType, null)
            };

        private IArchiveWriter CreateArchiveWriter(InternalArchiveType archiveType, WriterOptions options) =>
            new ArchiveWriter(_fileService, archiveType, options);
    }
}