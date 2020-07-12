using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Camelot.Extensions;
using Camelot.FileSystemWatcher.Configuration;
using Camelot.FileSystemWatcher.Interfaces;
using Camelot.Services.Abstractions;

namespace Camelot.FileSystemWatcher.Implementations
{
    public class AggregatingFileSystemWatcherDecorator : IFileSystemWatcher
    {
        private readonly IPathService _pathService;
        private readonly IFileSystemWatcher _fileSystemWatcher;
        private readonly ConcurrentQueue<FileSystemEventArgs> _eventsQueue;
        private readonly Timer _timer;

        public event EventHandler<FileSystemEventArgs> Created;

        public event EventHandler<FileSystemEventArgs> Changed;

        public event EventHandler<FileSystemEventArgs> Deleted;

        public event EventHandler<RenamedEventArgs> Renamed;

        public AggregatingFileSystemWatcherDecorator(
            IPathService pathService,
            IFileSystemWatcher fileSystemWatcher,
            FileSystemWatcherConfiguration configuration)
        {
            _pathService = pathService;
            _fileSystemWatcher = fileSystemWatcher;
            _eventsQueue = new ConcurrentQueue<FileSystemEventArgs>();
            _timer = new Timer(configuration.RefreshIntervalMs);

            SubscribeToEvents();
            StartTimer();
        }

        public void StartRaisingEvents() => _fileSystemWatcher.StartRaisingEvents();

        public void StopRaisingEvents() => _fileSystemWatcher.StopRaisingEvents();

        public void Dispose()
        {
            UnsubscribeFromEvents();
            StopTimer();

            _timer.Dispose();
            _fileSystemWatcher.Dispose();
        }

        private void StartTimer() => _timer.Start();

        private void StopTimer() => _timer.Stop();

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            var events = GetEvents();

            FireEvents(events);
        }

        private IEnumerable<FileSystemEventArgs> GetEvents()
        {
            // key: current file name, value: aggregation event
            var filesDictionary = new Dictionary<string, FileSystemEventArgs>();

            while (_eventsQueue.TryDequeue(out var fileSystemEventArgs))
            {
                var filePath = GetFullPath(fileSystemEventArgs);
                if (!filesDictionary.ContainsKey(filePath)) // always add first event
                {
                    filesDictionary[fileSystemEventArgs.FullPath] = fileSystemEventArgs;

                    continue;
                }

                // not first event
                var previousEventType = filesDictionary[filePath].ChangeType;
                var currentEventType = fileSystemEventArgs.ChangeType;

                switch (previousEventType, currentEventType)
                {
                    case (WatcherChangeTypes.Created, WatcherChangeTypes.Renamed): // create + rename = create with new name
                    {
                        var directory = _pathService.GetParentDirectory(fileSystemEventArgs.FullPath);
                        var args = new FileSystemEventArgs(WatcherChangeTypes.Created, directory, fileSystemEventArgs.Name);
                        filesDictionary.Remove(filePath);
                        filesDictionary[fileSystemEventArgs.FullPath] = args;
                        break;
                    }
                    case (WatcherChangeTypes.Created, WatcherChangeTypes.Deleted): // create + delete = null
                    {
                        filesDictionary.Remove(filePath);
                        break;
                    }
                    case (WatcherChangeTypes.Renamed, WatcherChangeTypes.Deleted): // rename + delete = delete with old name
                    {
                        var directory = _pathService.GetParentDirectory(filePath);
                        var renamedArgs = (RenamedEventArgs) filesDictionary[filePath];
                        var args = new FileSystemEventArgs(WatcherChangeTypes.Deleted, directory, renamedArgs.OldName);
                        filesDictionary.Remove(filePath);
                        filesDictionary[renamedArgs.OldFullPath] = args;
                        break;
                    }
                    case (WatcherChangeTypes.Changed, WatcherChangeTypes.Deleted): // change + delete = delete
                    {
                        filesDictionary[filePath] = fileSystemEventArgs;
                        break;
                    }
                    case (WatcherChangeTypes.Changed, WatcherChangeTypes.Renamed): // change + rename = rename
                    {
                        filesDictionary.Remove(filePath); // remove events for old path
                        filesDictionary[fileSystemEventArgs.FullPath] = fileSystemEventArgs;
                        break;
                    }
                    case (WatcherChangeTypes.Deleted, WatcherChangeTypes.Created): // delete + create = change
                    {
                        var directory = _pathService.GetParentDirectory(filePath);
                        var args = new FileSystemEventArgs(WatcherChangeTypes.Changed, directory, fileSystemEventArgs.Name);
                        filesDictionary[filePath] = args;
                        break;
                    }
                    case (WatcherChangeTypes.Renamed, WatcherChangeTypes.Renamed): // rename + rename = rename with old and new names
                    {
                        var directory = _pathService.GetParentDirectory(filePath);
                        var previousRenamedArgs = (RenamedEventArgs) filesDictionary[filePath];
                        var currentRenamedArgs = (RenamedEventArgs) fileSystemEventArgs;
                        var newName = currentRenamedArgs.Name;
                        var oldName = previousRenamedArgs.OldName;
                        var args = new RenamedEventArgs(WatcherChangeTypes.Renamed, directory, newName, oldName);
                        filesDictionary.Remove(filePath);
                        filesDictionary[currentRenamedArgs.FullPath] = args;
                        break;
                    }
                }
            }

            return filesDictionary.Values;
        }

        private void FireEvents(IEnumerable<FileSystemEventArgs> events)
        {
            foreach (var args in events)
            {
                switch (args.ChangeType)
                {
                    case WatcherChangeTypes.Created:
                        Created.Raise(this, args);
                        break;
                    case WatcherChangeTypes.Deleted:
                        Deleted.Raise(this, args);
                        break;
                    case WatcherChangeTypes.Changed:
                        Changed.Raise(this, args);
                        break;
                    case WatcherChangeTypes.Renamed:
                        Renamed.Raise(this, (RenamedEventArgs) args);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(args.ChangeType));
                }
            }
        }

        private void SubscribeToEvents()
        {
            _timer.Elapsed += TimerOnElapsed;

            _fileSystemWatcher.Changed += FileSystemWatcherOnEventFired;
            _fileSystemWatcher.Created += FileSystemWatcherOnEventFired;
            _fileSystemWatcher.Deleted += FileSystemWatcherOnEventFired;
            _fileSystemWatcher.Renamed += FileSystemWatcherOnEventFired;
        }

        private void UnsubscribeFromEvents()
        {
            _fileSystemWatcher.Changed -= FileSystemWatcherOnEventFired;
            _fileSystemWatcher.Created -= FileSystemWatcherOnEventFired;
            _fileSystemWatcher.Deleted -= FileSystemWatcherOnEventFired;
            _fileSystemWatcher.Renamed -= FileSystemWatcherOnEventFired;

            _timer.Elapsed -= TimerOnElapsed;
        }

        private void FileSystemWatcherOnEventFired(object sender, FileSystemEventArgs args) =>
            _eventsQueue.Enqueue(args);

        private static string GetFullPath(FileSystemEventArgs args) =>
            args is RenamedEventArgs renamedEventArgs
                ? renamedEventArgs.OldFullPath
                : args.FullPath;
    }
}