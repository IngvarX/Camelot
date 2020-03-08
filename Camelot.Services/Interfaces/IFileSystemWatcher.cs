namespace Camelot.Services.Interfaces
{
    public interface IFileSystemWatcher
    {
        void WatchDirectory(string directory);
        
        void UnwatchDirectory(string directory);
    }
}