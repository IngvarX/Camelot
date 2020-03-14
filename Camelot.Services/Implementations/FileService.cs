using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private static FileModel CreateFrom(string file)
        {
            var fileInfo = new FileInfo(file);
            var fileModel = new FileModel
            {
                Name = fileInfo.Name,
                FullPath = fileInfo.FullName,
                LastModifiedDateTime = fileInfo.LastWriteTime,
                Type = GetFileType(fileInfo)
            };

            return fileModel;
        }

        private static FileType GetFileType(FileSystemInfo fileInfo)
        {
            return fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint) ? FileType.Link : FileType.RegularFile;
        }
    }
}