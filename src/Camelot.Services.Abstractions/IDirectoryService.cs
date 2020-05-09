using System;
using System.Collections.Generic;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions
{
    public interface IDirectoryService
    {
        event EventHandler<SelectedDirectoryChangedEventArgs> SelectedDirectoryChanged;

        string SelectedDirectory { get; set; }

        bool Create(string directory);
        
        DirectoryModel GetParentDirectory(string directory);
        
        IReadOnlyCollection<DirectoryModel> GetDirectories(IReadOnlyCollection<string> directories);

        IReadOnlyCollection<DirectoryModel> GetChildDirectories(string directory);

        bool CheckIfExists(string directory);

        string GetAppRootDirectory();

        IReadOnlyCollection<string> GetFilesRecursively(string directory);

        void RemoveRecursively(string directory);

        void Rename(string directoryPath, string newName);
    }
}