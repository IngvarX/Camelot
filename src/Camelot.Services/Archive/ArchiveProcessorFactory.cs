using System;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Archive
{
    public class ArchiveProcessorFactory : IArchiveProcessorFactory
    {
        public IArchiveProcessor Create(ArchiveType archiveType)
        {
            return archiveType switch
            {
                ArchiveType.Zip => null,
                ArchiveType.Tar => null,
                ArchiveType.TarGz => null,
                ArchiveType.GZip => null,
                _ => throw new ArgumentOutOfRangeException(nameof(archiveType), archiveType.ToString())
            };
        }
    }
}