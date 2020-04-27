using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.Services.Interfaces
{
    public interface IOperationsService
    {
        void EditFiles(IReadOnlyCollection<string> files);

        Task CopyFilesAsync(IReadOnlyCollection<string> files, string destinationDirectory);

        Task MoveFilesAsync(IReadOnlyCollection<string> files, string destinationDirectory);

        void CreateDirectory(string sourceDirectory, string directoryName);

        Task RemoveFilesAsync(IReadOnlyCollection<string> files);
        
        Task RemoveFilesToTrashAsync(IReadOnlyCollection<string> files);

        void Rename(string path, string newName);
    }
}