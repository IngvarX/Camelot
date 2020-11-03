using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;

namespace Camelot.Services.Abstractions
{
    public interface IFileService
    {
        IReadOnlyList<FileModel> GetFiles(string directory, ISpecification<FileModel> specification = null);

        IReadOnlyList<FileModel> GetFiles(IReadOnlyList<string> files);

        FileModel GetFile(string file);

        bool CheckIfExists(string file);

        Task<bool> CopyAsync(string source, string destination, bool overwrite = false);

        bool Remove(string file);

        bool Rename(string filePath, string newName);

        Task WriteTextAsync(string filePath, string text);

        Task WriteBytesAsync(string filePath, byte[] bytes);

        void CreateFile(string filePath);

        Stream OpenRead(string filePath);

        Stream OpenWrite(string filePath);
    }
}