using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface IFileService
    {
        // TODO: pass ISpecification
        IReadOnlyList<FileModel> GetFiles(string directory);

        IReadOnlyList<FileModel> GetFiles(IReadOnlyList<string> files);

        FileModel GetFile(string file);

        bool CheckIfExists(string file);

        Task CopyAsync(string source, string destination, bool overwrite = false);

        void Remove(string file);

        bool Rename(string filePath, string newName);

        Task WriteTextAsync(string filePath, string text);

        Task WriteBytesAsync(string filePath, byte[] bytes);
    }
}