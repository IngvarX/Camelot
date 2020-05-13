using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;

namespace Camelot.Services
{
    public class FilesSelectionService : IFilesSelectionService
    {
        private readonly HashSet<string> _selectedFiles;

        public IReadOnlyList<string> SelectedFiles => _selectedFiles.ToArray();

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