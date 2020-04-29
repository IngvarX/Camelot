using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Models;

namespace Camelot.Services.Interfaces
{
    public interface IFileService
    {
        // TODO: pass ISpecification
        IReadOnlyCollection<FileModel> GetFiles(string directory);
        
        FileModel GetFile(string file);

        bool CheckIfExists(string file);

        Task CopyAsync(string source, string destination);

        void Remove(string file);

        void Rename(string filePath, string newName);

        Task WriteTextAsync(string filePath, string text);
        
        Task WriteBytesAsync(string filePath, byte[] bytes);
    }
}