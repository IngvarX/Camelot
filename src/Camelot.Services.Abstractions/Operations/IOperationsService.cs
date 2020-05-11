using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperationsService
    {
        void OpenFiles(IReadOnlyCollection<string> files);

        Task CopyFilesAsync(IReadOnlyCollection<string> files, string destinationDirectory);

        Task MoveFilesAsync(IReadOnlyCollection<string> files, string destinationDirectory);

        Task MoveFilesAsync(IDictionary<string, string> files);

        void CreateDirectory(string sourceDirectory, string directoryName);

        Task RemoveFilesAsync(IReadOnlyCollection<string> files);

        Task RemoveFilesToTrashAsync(IReadOnlyCollection<string> files);

        void Rename(string path, string newName);
    }
}