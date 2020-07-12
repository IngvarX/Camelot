using System;
using System.Collections.Generic;
using System.IO;
using Camelot.Extensions;
using Camelot.FileSystemWatcher.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services
{
    public class FileSystemWatchingService : IFileSystemWatchingService
    {
        private readonly IFileSystemWatcherFactory _fileSystemWatcherWrapperFactory;

        private readonly IDictionary<string, (IFileSystemWatcher, int)> _fileSystemWatchersDictionary;

        public event EventHandler<FileDeletedEventArgs> NodeDeleted;

        public event EventHandler<FileCreatedEventArgs> NodeCreated;

        public event EventHandler<FileChangedEventArgs> NodeChanged;

        public event EventHandler<FileRenamedEventArgs> NodeRenamed;

        public FileSystemWatchingService(IFileSystemWatcherFactory fileSystemWatcherWrapperFactory)
        {
            _fileSystemWatcherWrapperFactory = fileSystemWatcherWrapperFactory;

            _fileSystemWatchersDictionary = new Dictionary<string, (IFileSystemWatcher, int)>();
        }

        public void StartWatching(string directory)
        {
            if (_fileSystemWatchersDictionary.ContainsKey(directory))
            {
                var (watcher, subscriptionsCount) = _fileSystemWatchersDictionary[directory];
                _fileSystemWatchersDictionary[directory] = (watcher, subscriptionsCount + 1);
            }
            else
            {
                var fileSystemWatcher = _fileSystemWatcherWrapperFactory.Create(directory);
                _fileSystemWatchersDictionary[directory] = (fileSystemWatcher, 1);

                SubscribeToEvents(fileSystemWatcher);
            }
        }

        public void StopWatching(string directory)
        {
            if (!_fileSystemWatchersDictionary.ContainsKey(directory))
            {
                return;
            }

            var (watcher, subscriptionsCount) = _fileSystemWatchersDictionary[directory];
            if (subscriptionsCount == 1)
            {
                _fileSystemWatchersDictionary.Remove(directory);
                CleanupFileSystemWatcher(watcher);
            }
            else
            {
                _fileSystemWatchersDictionary[directory] = (watcher, subscriptionsCount - 1);
            }
        }

        private void CleanupFileSystemWatcher(IFileSystemWatcher fileSystemWatcher)
        {
            UnsubscribeFromEvents(fileSystemWatcher);

            fileSystemWatcher.Dispose();
        }

        private void SubscribeToEvents(IFileSystemWatcher fileSystemWatcher)
        {
            fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            fileSystemWatcher.Created += FileSystemWatcherOnCreated;
            fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;
            fileSystemWatcher.Renamed += FileSystemWatcherOnRenamed;

            fileSystemWatcher.StartRaisingEvents();
        }

        private void UnsubscribeFromEvents(IFileSystemWatcher fileSystemWatcher)
        {
            fileSystemWatcher.StopRaisingEvents();

            fileSystemWatcher.Changed -= FileSystemWatcherOnChanged;
            fileSystemWatcher.Created -= FileSystemWatcherOnCreated;
            fileSystemWatcher.Deleted -= FileSystemWatcherOnDeleted;
            fileSystemWatcher.Renamed -= FileSystemWatcherOnRenamed;
        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            var args = new FileChangedEventArgs(e.FullPath);

            NodeChanged.Raise(this, args);
        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            var args = new FileCreatedEventArgs(e.FullPath);

            NodeCreated.Raise(this, args);
        }

        private void FileSystemWatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            var args = new FileDeletedEventArgs(e.FullPath);

            NodeDeleted.Raise(this, args);
        }

        private void FileSystemWatcherOnRenamed(object sender, RenamedEventArgs e)
        {
            var args = new FileRenamedEventArgs(e.OldFullPath, e.FullPath);

            NodeRenamed.Raise(this, args);
        }
    }
}