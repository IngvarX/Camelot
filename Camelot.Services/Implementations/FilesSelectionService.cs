using System.Collections.Generic;
using Camelot.Services.Interfaces;
using Camelot.Services.Models;

namespace Camelot.Services.Implementations
{
    public class FilesSelectionService : IFilesSelectionService
    {
        private readonly List<FileModel> _selectedFiles;

        public IList<FileModel> SelectedFiles => _selectedFiles;

        public FilesSelectionService()
        {
            _selectedFiles = new List<FileModel>();
        }

        public void SelectFiles(IEnumerable<FileModel> files)
        {
            _selectedFiles.AddRange(files);
        }

        public void UnselectFiles(IEnumerable<FileModel> files)
        {
            foreach (var file in files)
            {
                _selectedFiles.Remove(file);
            }
        }
    }
}