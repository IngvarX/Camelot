using System;
using System.IO;
using Camelot.FileSystemWatcherWrapper.Interfaces;

namespace Camelot.FileSystemWatcherWrapper.Implementations
{
    public class FileSystemWatcherWrapper : IFileSystemWatcherWrapper
    {
        private readonly FileSystemWatcher _fileSystemWatcher;

        public event FileSystemEventHandler Created;
        public event FileSystemEventHandler Changed;
        public event FileSystemEventHandler Deleted;
        public event RenamedEventHandler Renamed;

        public bool EnableRaisingEvents
        {
            get => _fileSystemWatcher.EnableRaisingEvents;
            set => _fileSystemWatcher.EnableRaisingEvents = value;
        }

        public FileSystemWatcherWrapper(FileSystemWatcher fileSystemWatcher)
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
            _fileSystemWatcher.Created += Created;
            _fileSystemWatcher.Changed += Changed;
            _fileSystemWatcher.Deleted += Deleted;
            _fileSystemWatcher.Renamed += Renamed;
        }

        private void UnsubscribeFromEvents()
        {
            _fileSystemWatcher.Created -= Created;
            _fileSystemWatcher.Changed -= Changed;
            _fileSystemWatcher.Deleted -= Deleted;
            _fileSystemWatcher.Renamed -= Renamed;
        }
    }
}