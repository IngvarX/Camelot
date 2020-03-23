using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Interfaces;
using Camelot.Services.Models;

namespace Camelot.Services.Implementations
{
    public class FileService : IFileService
    {
        public IReadOnlyCollection<FileModel> GetFiles(string directory)
        {
            var files = Directory
                .GetFiles(directory)
                .Select(CreateFrom);

            return files.ToArray();
        }

        public bool CheckIfFileExists(string file)
        {
            return File.Exists(file);
        }

        public Task CopyFileAsync(string source, string destination)
        {
            File.Copy(source, destination);

            return Task.CompletedTask;
        }

        public void RemoveFile(string file)
        {
            File.Delete(file);
        }

        private static FileModel CreateFrom(string file)
        {
            var fileInfo = new FileInfo(file);
            var fileModel = new FileModel
            {
                Name = fileInfo.Name,
                FullPath = fileInfo.FullName,
                LastModifiedDateTime = fileInfo.LastWriteTime,
                Type = GetFileType(fileInfo),
                SizeBytes = fileInfo.Length,
                LastWriteTime = fileInfo.LastWriteTime
            };

            return fileModel;
        }

        private static FileType GetFileType(FileSystemInfo fileInfo)
        {
            return fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint) ? FileType.Link : FileType.RegularFile;
        }
    }
}