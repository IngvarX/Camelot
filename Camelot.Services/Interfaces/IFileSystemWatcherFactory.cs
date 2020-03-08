namespace Camelot.Services.Interfaces
{
    public interface IFileSystemWatcherFactory
    {
        IFileSystemWatcher Create(string directory);
    }
}