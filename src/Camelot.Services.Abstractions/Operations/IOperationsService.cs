using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperationsService
    {
        void OpenFiles(IReadOnlyList<string> nodes);

        Task CopyAsync(IReadOnlyList<string> nodes, string destinationDirectory);

        Task MoveAsync(IReadOnlyList<string> nodes, string destinationDirectory);

        Task MoveAsync(IReadOnlyDictionary<string, string> nodes);

        Task PackAsync(IReadOnlyList<string> nodes, string outputFilePath, ArchiveType archiveType);

        Task ExtractAsync(string archivePath, string outputDirectory, ArchiveType archiveType);

        void CreateDirectory(string sourceDirectory, string directoryName);

        void CreateFile(string sourceDirectory, string fileName);

        Task RemoveAsync(IReadOnlyList<string> nodes);

        bool Rename(string path, string newName);
    }
}