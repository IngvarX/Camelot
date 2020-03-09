using System;
using System.IO;

namespace Camelot.FileSystemWatcherWrapper.Interfaces
{
    public interface IFileSystemWatcherWrapper : IDisposable
    {
        public event FileSystemEventHandler Created;

        public event FileSystemEventHandler Changed;

        public event FileSystemEventHandler Deleted;

        public event RenamedEventHandler Renamed;

        public bool EnableRaisingEvents { get; set; }
    }
}