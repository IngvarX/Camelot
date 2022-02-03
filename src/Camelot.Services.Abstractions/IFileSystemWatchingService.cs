using System;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions;

public interface IFileSystemWatchingService
{
    event EventHandler<FileDeletedEventArgs> NodeDeleted;

    event EventHandler<FileCreatedEventArgs> NodeCreated;

    event EventHandler<FileChangedEventArgs> NodeChanged;

    event EventHandler<FileRenamedEventArgs> NodeRenamed;

    void StartWatching(string directory);

    void StopWatching(string directory);
}