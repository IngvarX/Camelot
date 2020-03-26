using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Models;

namespace Camelot.Services.Interfaces
{
    public interface IFileService
    {
        // TODO: pass ISpecification
        IReadOnlyCollection<FileModel> GetFiles(string directory);

        bool CheckIfFileExists(string file);

        Task CopyFileAsync(string source, string destination);

        void RemoveFile(string file);
    }
}