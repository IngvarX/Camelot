using System.IO;
using Camelot.FileSystemWatcherWrapper.Configuration;
using Camelot.FileSystemWatcherWrapper.Interfaces;
using Camelot.Services.Abstractions;

namespace Camelot.FileSystemWatcherWrapper.Implementations
{
    public class FileSystemWatcherFactory : IFileSystemWatcherFactory
    {
        private readonly IPathService _pathService;
        private readonly FileSystemWatcherConfiguration _fileSystemWatcherConfiguration;

        public FileSystemWatcherFactory(
            IPathService pathService,
            FileSystemWatcherConfiguration fileSystemWatcherConfiguration)
        {
            _pathService = pathService;
            _fileSystemWatcherConfiguration = fileSystemWatcherConfiguration;
        }

        public IFileSystemWatcher Create(string directory)
        {
            var fileSystemWatcher = new FileSystemWatcher
            {
                Path = directory,
                NotifyFilter = NotifyFilters.Attributes |
                               NotifyFilters.DirectoryName |
                               NotifyFilters.FileName |
                               NotifyFilters.LastWrite |
                               NotifyFilters.Security |
                               NotifyFilters.Size
            };

            var wrapper = new FileSystemWatcherAdapter(fileSystemWatcher);

            return new AggregatingFileSystemWatcherDecorator(_pathService, wrapper, _fileSystemWatcherConfiguration);
        }
    }
}