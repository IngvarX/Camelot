using System.Collections.Generic;
using Camelot.Services.Models;

namespace Camelot.Services.Interfaces
{
    public interface IDirectoryService
    {
        string SelectedDirectory { get; set; }

        bool CreateDirectory(string directory);

        IReadOnlyCollection<DirectoryModel> GetDirectories(string directory);
    }
}