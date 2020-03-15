using System;
using System.Collections.Generic;
using Camelot.Services.EventArgs;
using Camelot.Services.Models;

namespace Camelot.Services.Interfaces
{
    public interface IDirectoryService
    {
        event EventHandler<SelectedDirectoryChangedEventArgs> SelectedDirectoryChanged;

        string SelectedDirectory { get; set; }

        bool CreateDirectory(string directory);

        IReadOnlyCollection<DirectoryModel> GetDirectories(string directory);

        bool DirectoryExists(string directory);
    }
}