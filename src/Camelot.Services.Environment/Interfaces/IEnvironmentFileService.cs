using System.IO;
using System.Threading.Tasks;

namespace Camelot.Services.Environment.Interfaces
{
    public interface IEnvironmentFileService
    {
        FileInfo GetFile(string file);

        string[] GetFiles(string directory);

        bool CheckIfExists(string filePath);

        void Move(string oldFilePath, string newFilePath);

        void Delete(string filePath);

        Task WriteTextAsync(string filePath, string text);

        Task WriteBytesAsync(string filePath, byte[] bytes);

        void Create(string filePath);

        Stream OpenRead(string filePath);

        Stream OpenWrite(string filePath);
    }
}