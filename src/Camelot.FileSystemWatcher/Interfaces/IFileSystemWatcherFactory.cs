namespace Camelot.FileSystemWatcherWrapper.Interfaces
{
    public interface IFileSystemWatcherFactory
    {
        IFileSystemWatcher Create(string directory);
    }
}