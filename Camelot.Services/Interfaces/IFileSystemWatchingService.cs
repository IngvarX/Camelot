using System;
using Camelot.Services.EventArgs;

namespace Camelot.Services.Interfaces
{
    public interface IFileSystemWatchingService
    {
        event EventHandler<FileDeletedEventArgs> FileDeleted;
        event EventHandler<FileCreatedEventArgs> FileCreated;
        event EventHandler<FileChangedEventArgs> FileChanged;
        event EventHandler<FileRenamedEventArgs> FileRenamed;

        void StartWatching(string directory);

        void StopWatching();
    }
}