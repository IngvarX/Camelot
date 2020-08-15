using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Specifications;

namespace Camelot.Services
{
    public class FileService : IFileService
    {
        private readonly IPathService _pathService;

        public FileService(IPathService pathService)
        {
            _pathService = pathService;
        }

        public IReadOnlyList<FileModel> GetFiles(string directory, ISpecification<FileModel> specification) =>
            Directory
                .GetFiles(directory)
                .Select(CreateFrom)
                .Where(specification.IsSatisfiedBy)
                .ToArray();

        public IReadOnlyList<FileModel> GetFiles(IReadOnlyList<string> files) =>
            files.Select(CreateFrom).ToArray();

        public FileModel GetFile(string file) => CreateFrom(file);

        public bool CheckIfExists(string file) => File.Exists(file);

        public Task CopyAsync(string source, string destination, bool overwrite)
        {
            File.Copy(source, destination, overwrite);

            return Task.CompletedTask;
        }

        public void Remove(string file) => File.Delete(file);

        public bool Rename(string filePath, string newName)
        {
            var parentDirectory = _pathService.GetParentDirectory(filePath);
            var newFilePath = _pathService.Combine(parentDirectory, newName);

            try
            {
                File.Move(filePath, newFilePath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public Task WriteTextAsync(string filePath, string text) =>
            File.WriteAllTextAsync(filePath, text);

        public Task WriteBytesAsync(string filePath, byte[] bytes) =>
            File.WriteAllBytesAsync(filePath, bytes);

        private FileModel CreateFrom(string file)
        {
            var fileInfo = new FileInfo(file);
            var fileModel = new FileModel
            {
                Name = fileInfo.Name,
                FullPath = fileInfo.FullName,
                LastModifiedDateTime = fileInfo.LastWriteTime,
                Type = GetFileType(fileInfo),
                SizeBytes = fileInfo.Length,
                Extension = _pathService.GetExtension(fileInfo.Name),
                LastAccessDateTime = fileInfo.LastAccessTime,
                CreatedDateTime = fileInfo.CreationTime
            };

            return fileModel;
        }

        private static FileType GetFileType(FileSystemInfo fileInfo) =>
            fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint) ? FileType.Link : FileType.RegularFile;
    }
}