namespace Camelot.FileSystemWatcherWrapper.Interfaces
{
    public interface IFileSystemWatcherWrapperFactory
    {
        IFileSystemWatcherWrapper Create(string directory);
    }
}