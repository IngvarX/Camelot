using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.Services.Interfaces
{
    public interface IOperationsService
    {
        void OpenFiles(IReadOnlyCollection<string> files);

        Task CopyFilesAsync(IReadOnlyCollection<string> files, string destinationDirectory);

        Task MoveFilesAsync(IReadOnlyCollection<string> files, string destinationDirectory);
        
        Task MoveFilesAsync(IDictionary<string, string> files);

        void CreateDirectory(string sourceDirectory, string directoryName);

        Task RemoveFilesAsync(IReadOnlyCollection<string> files);

        void Rename(string path, string newName);
    }
}