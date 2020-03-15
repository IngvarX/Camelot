using System;
using System.IO;

namespace Camelot.FileSystemWatcherWrapper.Interfaces
{
    public interface IFileSystemWatcherWrapper : IDisposable
    {
        public event EventHandler<FileSystemEventArgs> Created;

        public event EventHandler<FileSystemEventArgs> Changed;

        public event EventHandler<FileSystemEventArgs> Deleted;

        public event EventHandler<RenamedEventArgs> Renamed;

        public bool EnableRaisingEvents { get; set; }
    }
}