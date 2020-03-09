using System;
using System.IO;
using Camelot.FileSystemWatcherWrapper.Interfaces;

namespace Camelot.FileSystemWatcherWrapper.Implementations
{
    public class FileSystemWatcherWrapperFactory : IFileSystemWatcherWrapperFactory
    {
        public IFileSystemWatcherWrapper Create(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }

            var fileSystemWatcher = new FileSystemWatcher(directory)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.Attributes |
                               NotifyFilters.CreationTime |
                               NotifyFilters.DirectoryName |
                               NotifyFilters.FileName |
                               NotifyFilters.LastAccess |
                               NotifyFilters.LastWrite |
                               NotifyFilters.Security |
                               NotifyFilters.Size
            };

            return new FileSystemWatcherWrapper(fileSystemWatcher);
        }
    }
}