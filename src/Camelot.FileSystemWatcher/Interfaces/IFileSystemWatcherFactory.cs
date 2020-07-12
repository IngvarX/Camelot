namespace Camelot.FileSystemWatcher.Interfaces
{
    public interface IFileSystemWatcherFactory
    {
        IFileSystemWatcher Create(string directory);
    }
}