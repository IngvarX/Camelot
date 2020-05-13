using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperationsService
    {
        void OpenFiles(IReadOnlyCollection<string> nodes);

        Task CopyAsync(IReadOnlyCollection<string> nodes, string destinationDirectory);

        Task MoveAsync(IReadOnlyCollection<string> nodes, string destinationDirectory);

        Task MoveAsync(IReadOnlyDictionary<string, string> nodes);

        void CreateDirectory(string sourceDirectory, string directoryName);

        Task RemoveAsync(IReadOnlyCollection<string> nodes);

        void Rename(string path, string newName);
    }
}