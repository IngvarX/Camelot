using System;
using System.IO;

namespace Camelot.FileSystemWatcher.Interfaces
{
    public interface IFileSystemWatcher : IDisposable
    {
        public event EventHandler<FileSystemEventArgs> Created;

        public event EventHandler<FileSystemEventArgs> Changed;

        public event EventHandler<FileSystemEventArgs> Deleted;

        public event EventHandler<RenamedEventArgs> Renamed;

        void StartRaisingEvents();

        void StopRaisingEvents();
    }
}