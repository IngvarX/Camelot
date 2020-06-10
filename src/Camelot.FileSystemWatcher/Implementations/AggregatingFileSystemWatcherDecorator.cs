using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Camelot.Extensions;
using Camelot.FileSystemWatcherWrapper.Configuration;
using Camelot.FileSystemWatcherWrapper.Interfaces;
using Camelot.Services.Abstractions;

namespace Camelot.FileSystemWatcherWrapper.Implementations
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

        public void StartRaisingEvents()
        {
            _fileSystemWatcher.StartRaisingEvents();
        }

        public void StopRaisingEvents()
        {
            _fileSystemWatcher.StopRaisingEvents();
        }

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
            var filesDictionary = new Dictionary<string, FileSystemEventArgs>();
            foreach (var fileSystemEventArgs in _eventsQueue)
            {
                var file = fileSystemEventArgs.FullPath;
                if (!filesDictionary.ContainsKey(file))
                {
                    filesDictionary[file] = fileSystemEventArgs;

                    continue;
                }

                var eventType = fileSystemEventArgs.ChangeType;
                switch (eventType)
                {
                    case WatcherChangeTypes.Created: // file was deleted and recreated == changed
                        var directory = _pathService.GetParentDirectory(file);
                        var args = new FileSystemEventArgs(WatcherChangeTypes.Changed, directory, fileSystemEventArgs.Name);
                        filesDictionary[file] = args;
                        break;
                    case WatcherChangeTypes.Deleted: // file was removed, overwrite previous events
                        filesDictionary[file] = fileSystemEventArgs;
                        break;
                    case WatcherChangeTypes.Changed: // file was changed
                        filesDictionary[file] = fileSystemEventArgs;
                        break;
                    case WatcherChangeTypes.Renamed: // file was renamed, overwrite previous events
                        filesDictionary[file] = fileSystemEventArgs;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(eventType));
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

        private void FileSystemWatcherOnEventFired(object sender, FileSystemEventArgs e) =>
            _eventsQueue.Enqueue(e);
    }
}