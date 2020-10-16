using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Archive
{
    public interface IArchiveTypeMapper
    {
        ArchiveType? GetArchiveTypeFrom(string filePath);
    }
}