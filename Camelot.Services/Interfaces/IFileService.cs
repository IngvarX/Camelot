using System.Collections.Generic;
using Camelot.Services.Models;

namespace Camelot.Services.Interfaces
{
    public interface IFileService
    {
        // TODO: pass ISpecification
        IReadOnlyCollection<FileModel> GetFiles(string directory);

        bool CheckIfFileExists(string file);

        void RemoveFile(string file);
    }
}