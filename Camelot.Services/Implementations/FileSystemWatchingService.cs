using System;
using System.IO;
using Camelot.Extensions;
using Camelot.FileSystemWatcherWrapper.Interfaces;
using Camelot.Services.EventArgs;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class FileSystemWatchingService : IFileSystemWatchingService, IDisposable
    {
        private readonly IFileSystemWatcherWrapperFactory _fileSystemWatcherWrapperFactory;

        private IFileSystemWatcherWrapper _fileSystemWatcher;

        public event EventHandler<NodeDeletedEventArgs> NodeDeleted;
        public event EventHandler<NodeCreatedEventArgs> NodeCreated;
        public event EventHandler<NodeChangedEventArgs> NodeChanged;
        public event EventHandler<NodeRenamedEventArgs> NodeRenamed;

        public FileSystemWatchingService(IFileSystemWatcherWrapperFactory fileSystemWatcherWrapperFactory)
        {
            _fileSystemWatcherWrapperFactory = fileSystemWatcherWrapperFactory;
        }

        public void StartWatching(string directory)
        {
            _fileSystemWatcher = _fileSystemWatcherWrapperFactory.Create(directory);

            SubscribeToEvents();
        }

        public void StopWatching()
        {
            CleanupFileSystemWatcher();
        }

        public void Dispose()
        {
            CleanupFileSystemWatcher();
        }

        private void CleanupFileSystemWatcher()
        {
            if (_fileSystemWatcher is null)
            {
                return;
            }

            UnsubscribeFromEvents();

            _fileSystemWatcher.Dispose();
            _fileSystemWatcher = null;
        }

        private void SubscribeToEvents()
        {
            _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
            _fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed += FileSystemWatcherOnRenamed;

            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void UnsubscribeFromEvents()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;

            _fileSystemWatcher.Changed -= FileSystemWatcherOnChanged;
            _fileSystemWatcher.Created -= FileSystemWatcherOnCreated;
            _fileSystemWatcher.Deleted -= FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed -= FileSystemWatcherOnRenamed;
        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            var args = new NodeChangedEventArgs(e.FullPath);

            NodeChanged.Raise(this, args);
        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            var args = new NodeCreatedEventArgs(e.FullPath);

            NodeCreated.Raise(this, args);
        }

        private void FileSystemWatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            var args = new NodeDeletedEventArgs(e.FullPath);

            NodeDeleted.Raise(this, args);
        }

        private void FileSystemWatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            var args = new NodeRenamedEventArgs(e.OldFullPath, e.FullPath);

            NodeRenamed.Raise(this, args);
        }
    }
}