using System.Collections.Generic;
using Camelot.Services.Models;

namespace Camelot.Services.Interfaces
{
    public interface IFilesSelectionService
    {
        IList<FileModel> SelectedFiles { get; }

        void SelectFiles(IEnumerable<FileModel> files);

        void UnselectFiles(IEnumerable<FileModel> files);
    }
}