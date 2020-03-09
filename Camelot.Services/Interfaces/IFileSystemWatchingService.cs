using System;
using Camelot.Services.EventArgs;

namespace Camelot.Services.Interfaces
{
    public interface IFileSystemWatchingService
    {
        event EventHandler<NodeDeletedEventArgs> NodeDeleted;
        event EventHandler<NodeCreatedEventArgs> NodeCreated;
        event EventHandler<NodeChangedEventArgs> NodeChanged;
        event EventHandler<NodeRenamedEventArgs> NodeRenamed;

        void StartWatching(string directory);

        void StopWatching();
    }
}