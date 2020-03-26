using System.Collections.Generic;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class FilesSelectionService : IFilesSelectionService
    {
        private readonly HashSet<string> _selectedFiles;

        public IReadOnlyCollection<string> SelectedFiles => _selectedFiles;

        public FilesSelectionService()
        {
            _selectedFiles = new HashSet<string>();
        }

        public void SelectFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                _selectedFiles.Add(file);
            }
        }

        public void UnselectFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                _selectedFiles.Remove(file);
            }
        }
    }
}