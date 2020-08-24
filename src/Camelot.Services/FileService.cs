using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Specifications;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services
{
    public class FileService : IFileService
    {
        private readonly IPathService _pathService;
        private readonly IEnvironmentFileService _environmentFileService;

        public FileService(
            IPathService pathService,
            IEnvironmentFileService environmentFileService)
        {
            _pathService = pathService;
            _environmentFileService = environmentFileService;
        }

        public IReadOnlyList<FileModel> GetFiles(string directory, ISpecification<FileModel> specification) =>
            _environmentFileService
                .GetFiles(directory)
                .Select(CreateFrom)
                .WhereNotNull()
                .Where(specification.IsSatisfiedBy)
                .ToArray();

        public IReadOnlyList<FileModel> GetFiles(IReadOnlyList<string> files) =>
            files.Select(CreateFrom).WhereNotNull().ToArray();

        public FileModel GetFile(string file) => CreateFrom(file);

        public bool CheckIfExists(string file) => _environmentFileService.CheckIfExists(file);

        public Task CopyAsync(string source, string destination, bool overwrite)
        {
            _environmentFileService.Copy(source, destination, overwrite);

            return Task.CompletedTask;
        }

        public void Remove(string file) => _environmentFileService.Delete(file);

        public bool Rename(string filePath, string newName)
        {
            var parentDirectory = _pathService.GetParentDirectory(filePath);
            var newFilePath = _pathService.Combine(parentDirectory, newName);

            try
            {
                _environmentFileService.Move(filePath, newFilePath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public Task WriteTextAsync(string filePath, string text) =>
            _environmentFileService.WriteTextAsync(filePath, text);

        public Task WriteBytesAsync(string filePath, byte[] bytes) =>
            _environmentFileService.WriteBytesAsync(filePath, bytes);

        public void CreateFile(string filePath) =>
            _environmentFileService.Create(filePath);

        private FileModel CreateFrom(string file)
        {
            try
            {
                var fileInfo = _environmentFileService.GetFile(file);
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
            catch
            {
                return null;
            }
        }

        private static FileType GetFileType(FileSystemInfo fileInfo) =>
            fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint) ? FileType.Link : FileType.RegularFile;
    }
}