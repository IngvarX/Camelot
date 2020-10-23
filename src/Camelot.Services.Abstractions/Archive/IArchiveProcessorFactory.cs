using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Archive
{
    public interface IArchiveProcessorFactory
    {
        IArchiveReader CreateReader(ArchiveType archiveType);

        IArchiveWriter CreateWriter(ArchiveType archiveType);
    }
}