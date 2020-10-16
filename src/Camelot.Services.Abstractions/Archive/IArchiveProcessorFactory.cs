using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Archive
{
    public interface IArchiveProcessorFactory
    {
        IArchiveProcessor Create(ArchiveType archiveType);
    }
}