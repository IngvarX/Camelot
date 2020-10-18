using System;
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
using Microsoft.Extensions.Logging;

namespace Camelot.Services
{
    public class FileService : IFileService
    {
        private readonly IPathService _pathService;
        private readonly IEnvironmentFileService _environmentFileService;
        private readonly ILogger _logger;

        public FileService(
            IPathService pathService,
            IEnvironmentFileService environmentFileService,
            ILogger logger)
        {
            _pathService = pathService;
            _environmentFileService = environmentFileService;
            _logger = logger;
        }

        public IReadOnlyList<FileModel> GetFiles(string directory, ISpecification<FileModel> specification = null) =>
            _environmentFileService
                .GetFiles(directory)
                .Select(CreateFrom)
                .WhereNotNull()
                .Where(f => specification?.IsSatisfiedBy(f) ?? true)
                .ToArray();

        public IReadOnlyList<FileModel> GetFiles(IReadOnlyList<string> files) =>
            files.Select(CreateFrom).WhereNotNull().ToArray();

        public FileModel GetFile(string file) => CreateFrom(file);

        public bool CheckIfExists(string file) => _environmentFileService.CheckIfExists(file);

        public Task<bool> CopyAsync(string source, string destination, bool overwrite)
        {
            try
            {
                _environmentFileService.Copy(source, destination, overwrite);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Failed to copy file {source} to {destination} (overwrite: {overwrite}) with error {ex}");

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public bool Remove(string file)
        {
            try
            {
                _environmentFileService.Delete(file);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to remove file {file} with error {ex}");

                return false;
            }

            return true;
        }

        public bool Rename(string filePath, string newName)
        {
            var parentDirectory = _pathService.GetParentDirectory(filePath);
            var newFilePath = _pathService.Combine(parentDirectory, newName);

            try
            {
                _environmentFileService.Move(filePath, newFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to rename file {filePath} to {newName} with error {ex}");

                return false;
            }

            return true;
        }

        public Task WriteTextAsync(string filePath, string text) =>
            _environmentFileService.WriteTextAsync(filePath, text);

        public Task WriteBytesAsync(string filePath, byte[] bytes) =>
            _environmentFileService.WriteBytesAsync(filePath, bytes);

        public void CreateFile(string filePath)
        {
            try
            {
                _environmentFileService.Create(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create file {filePath} with error {ex}");
            }
        }

        public FileStream OpenRead(string filePath) => _environmentFileService.OpenRead(filePath);

        public FileStream OpenWrite(string filePath) => _environmentFileService.OpenWrite(filePath);

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