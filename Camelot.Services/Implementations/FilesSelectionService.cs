using System.Collections.Generic;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class FilesSelectionService : IFilesSelectionService
    {
        public ISet<string> SelectedFiles { get; }

        public FilesSelectionService()
        {
            SelectedFiles = new HashSet<string>();
        }

        public void SelectFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                SelectedFiles.Add(file);
            }
        }

        public void UnselectFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                SelectedFiles.Remove(file);
            }
        }

        public void ClearSelectedFiles()
        {
            SelectedFiles.Clear();
        }
    }
}