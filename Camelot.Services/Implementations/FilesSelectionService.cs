using System.Collections.Generic;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class FilesSelectionService : IFilesSelectionService
    {
        private readonly List<string> _selectedFiles;

        public IList<string> SelectedFiles => _selectedFiles;

        public FilesSelectionService()
        {
            _selectedFiles = new List<string>();
        }

        public void SelectFiles(IEnumerable<string> files)
        {
            _selectedFiles.AddRange(files);
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