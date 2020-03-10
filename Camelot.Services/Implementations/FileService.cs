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
            var directories = GetDirectories(directory);
            var regularFiles = GetRegularFiles(directory);

            return directories.Concat(regularFiles).ToArray();
        }

        private static IEnumerable<FileModel> GetDirectories(string directory)
        {
            var directories = Directory
                .GetDirectories(directory)
                .Select(d => CreateFrom(new DirectoryInfo(d)));

            return directories;
        }

        private static IEnumerable<FileModel> GetRegularFiles(string directory)
        {
            var directories = Directory
                .GetFiles(directory)
                .Select(f => CreateFrom(new FileInfo(f)));

            return directories;
        }

        private static FileModel CreateFrom(FileSystemInfo directory)
        {
            var fileModel = new FileModel
            {
                Name = directory.Name,
                LastModifiedDateTime = directory.LastWriteTime,
                Type = NodeType.Directory
            };

            return fileModel;
        }

        private static FileModel CreateFrom(FileInfo file)
        {
            var fileModel = new FileModel
            {
                Name = file.Name,
                LastModifiedDateTime = file.LastWriteTime,
                Type = NodeType.RegularFile
            };

            return fileModel;
        }
    }
}