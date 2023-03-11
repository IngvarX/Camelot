using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Operations;

public interface IOperationsService
{
    void OpenFiles(IReadOnlyList<string> nodes);

    Task<bool> CopyAsync(IReadOnlyList<string> nodes, string destinationDirectory);

    Task<bool> MoveAsync(IReadOnlyList<string> nodes, string destinationDirectory);

    Task<bool> MoveAsync(IReadOnlyDictionary<string, string> nodes);

    Task<bool> PackAsync(IReadOnlyList<string> nodes, string outputFilePath, ArchiveType archiveType);

    Task<bool> ExtractAsync(string archivePath, string outputDirectory, ArchiveType archiveType);

    void CreateDirectory(string sourceDirectory, string directoryName);

    void CreateFile(string sourceDirectory, string fileName);

    Task<bool> RemoveAsync(IReadOnlyList<string> nodes);

    bool Rename(string path, string newName);
}