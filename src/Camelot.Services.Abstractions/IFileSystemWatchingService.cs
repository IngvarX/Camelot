using System;
using Camelot.Services.Models.EventArgs;

namespace Camelot.Services.Abstractions
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