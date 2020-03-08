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

            SetupFileSystemWatcher();
        }

        public void WatchDirectory(string directory)
        {
            throw new System.NotImplementedException();
        }

        public void UnwatchDirectory(string directory)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
        }

        private void SetupFileSystemWatcher()
        {
            _fileSystemWatcher.IncludeSubdirectories = true;
            _fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes |
                                              NotifyFilters.CreationTime |
                                              NotifyFilters.DirectoryName |
                                              NotifyFilters.FileName |
                                              NotifyFilters.LastAccess |
                                              NotifyFilters.LastWrite |
                                              NotifyFilters.Security |
                                              NotifyFilters.Size;

            SubscribeToEvents();

            _fileSystemWatcher.EnableRaisingEvents = true;
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