namespace Camelot.Services.FileSystemWatcher.Interfaces;

public interface IFileSystemWatcherFactory
{
    IFileSystemWatcher Create(string directory);
}