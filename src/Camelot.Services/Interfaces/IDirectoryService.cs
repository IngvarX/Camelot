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
        
        DirectoryModel GetParentDirectory(string directory);

        IReadOnlyCollection<DirectoryModel> GetDirectories(string directory);

        bool CheckIfDirectoryExists(string directory);

        string GetAppRootDirectory();

        IReadOnlyCollection<string> GetFilesRecursively(string directory);

        void RemoveDirectoryRecursively(string directory);
    }
}