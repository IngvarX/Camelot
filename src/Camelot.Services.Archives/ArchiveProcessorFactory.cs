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

        public IArchiveProcessor Create(ArchiveType archiveType) =>
            archiveType switch
            {
                ArchiveType.Tar => CreateArchiveProcessor(InternalArchiveType.Tar, CompressionType.None),
                ArchiveType.Zip => CreateArchiveProcessor(InternalArchiveType.Zip, CompressionType.Deflate),
                ArchiveType.TarGz => CreateArchiveProcessor(InternalArchiveType.Tar, CompressionType.GZip),
                ArchiveType.GZip => CreateArchiveProcessor(InternalArchiveType.Zip, CompressionType.GZip),
                ArchiveType.TarBz2 => CreateArchiveProcessor(InternalArchiveType.Tar, CompressionType.BZip2),
                ArchiveType.Bz2 => CreateArchiveProcessor(InternalArchiveType.Zip, CompressionType.BZip2),
                ArchiveType.TarXz => CreateArchiveProcessor(InternalArchiveType.Tar, CompressionType.Xz),
                ArchiveType.Xz => CreateArchiveProcessor(InternalArchiveType.Zip, CompressionType.Xz),
                ArchiveType.TarLz => CreateArchiveProcessor(InternalArchiveType.Tar, CompressionType.LZip),
                ArchiveType.Lz => CreateArchiveProcessor(InternalArchiveType.Zip, CompressionType.LZip),
                ArchiveType.SevenZip => CreateArchiveProcessor(InternalArchiveType.SevenZip, CompressionType.LZMA),
                _ => throw new ArgumentOutOfRangeException(nameof(archiveType), archiveType, null)
            };

        private IArchiveProcessor CreateArchiveProcessor(InternalArchiveType archiveType, WriterOptions options) =>
            new ArchiveProcessor(_fileService, archiveType, options);
    }
}