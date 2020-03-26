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

        public event EventHandler<FileDeletedEventArgs> FileDeleted;

        public event EventHandler<FileCreatedEventArgs> FileCreated;

        public event EventHandler<FileChangedEventArgs> FileChanged;

        public event EventHandler<FileRenamedEventArgs> FileRenamed;

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
            var args = new FileChangedEventArgs(e.FullPath);

            FileChanged.Raise(this, args);
        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            var args = new FileCreatedEventArgs(e.FullPath);

            FileCreated.Raise(this, args);
        }

        private void FileSystemWatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            var args = new FileDeletedEventArgs(e.FullPath);

            FileDeleted.Raise(this, args);
        }

        private void FileSystemWatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            var args = new FileRenamedEventArgs(e.OldFullPath, e.FullPath);

            FileRenamed.Raise(this, args);
        }
    }
}