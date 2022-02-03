using System.IO;
using Camelot.Services.Abstractions;
using Camelot.Services.FileSystemWatcher.Configuration;
using Camelot.Services.FileSystemWatcher.Interfaces;

namespace Camelot.Services.FileSystemWatcher.Implementations;

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
        var fileSystemWatcher = new System.IO.FileSystemWatcher
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