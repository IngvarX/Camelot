using System;
using System.IO;
using Camelot.Services.Interfaces;
using IoFileSystemWatcher = System.IO.FileSystemWatcher;

namespace Camelot.Services.Implementations
{
    public class FileSystemWatcher : IFileSystemWatcher, IDisposable
    {
        private readonly IoFileSystemWatcher _fileSystemWatcher;

        public FileSystemWatcher(IoFileSystemWatcher fileSystemWatcher)
        {
            _fileSystemWatcher = fileSystemWatcher;

            SubscribeToEvents();
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();

            _fileSystemWatcher.Dispose();
        }

        private void SubscribeToEvents()
        {
            _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
            _fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed += FileSystemWatcherOnRenamed;
        }

        private void UnsubscribeFromEvents()
        {
            _fileSystemWatcher.Changed -= FileSystemWatcherOnChanged;
            _fileSystemWatcher.Created -= FileSystemWatcherOnCreated;
            _fileSystemWatcher.Deleted -= FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed -= FileSystemWatcherOnRenamed;
        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FileSystemWatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FileSystemWatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}