using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperationsService
    {
        void OpenFiles(IReadOnlyList<string> nodes);

        Task CopyAsync(IReadOnlyList<string> nodes, string destinationDirectory);

        Task MoveAsync(IReadOnlyList<string> nodes, string destinationDirectory);

        Task MoveAsync(IReadOnlyDictionary<string, string> nodes);

        void CreateDirectory(string sourceDirectory, string directoryName);

        Task RemoveAsync(IReadOnlyList<string> nodes);

        bool Rename(string path, string newName);
    }
}