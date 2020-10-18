using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Abstractions.Archive
{
    public interface IArchiveOperationsFactory
    {
        IOperation CreatePackOperation(IReadOnlyList<string> nodes, string outputFilePath, ArchiveType archiveType);

        IOperation CreateExtractOperation(string archivePath, string outputDirectory, ArchiveType archiveType);
    }
}