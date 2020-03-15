using System.IO;
using Camelot.FileSystemWatcherWrapper.Interfaces;

namespace Camelot.FileSystemWatcherWrapper.Implementations
{
    public class FileSystemWatcherWrapperFactory : IFileSystemWatcherWrapperFactory
    {
        public IFileSystemWatcherWrapper Create(string directory)
        {
            var fileSystemWatcher = new FileSystemWatcher
            {
                Path = directory,
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