using System;
using System.IO;
using Camelot.Services.Interfaces;
using IoFileSystemWatcher = System.IO.FileSystemWatcher;

namespace Camelot.Services.Implementations
{
    public class FileSystemWatcherFactory : IFileSystemWatcherFactory
    {
        public IFileSystemWatcher Create(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }

            var fileSystemWatcher = new IoFileSystemWatcher(directory)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.Attributes |
                               NotifyFilters.CreationTime |
                               NotifyFilters.DirectoryName |
                               NotifyFilters.FileName |
                               NotifyFilters.LastAccess |
                               NotifyFilters.LastWrite |
                               NotifyFilters.Security |
                               NotifyFilters.Size,
                EnableRaisingEvents = true
            };

            return new FileSystemWatcher(fileSystemWatcher);
        }
    }
}