using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Archive
{
    public interface IArchiveService
    {
        Task PackAsync(IReadOnlyList<string> nodes, string outputFile, ArchiveType archiveType);

        Task UnpackAsync(string archivePath, string outputDirectory = null);

        bool CheckIfNodeIsArchive(string nodePath);
    }
}