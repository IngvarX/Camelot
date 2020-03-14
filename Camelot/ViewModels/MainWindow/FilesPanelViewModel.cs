using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Camelot.Services.Interfaces;
using DynamicData;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class FilesPanelViewModel : ViewModelBase
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;

        private readonly ObservableCollection<FileViewModel> _files;

        private string _currentDirectory;

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                var directoryChanged = _currentDirectory != value;
                this.RaiseAndSetIfChanged(ref _currentDirectory, value);
                if (directoryChanged)
                {
                    ReloadFiles();
                }
            }
        }

        public IEnumerable<FileViewModel> Files => _files;

        public FilesPanelViewModel(
            IFileService fileService,
            IDirectoryService directoryService)
        {
            _fileService = fileService;
            _directoryService = directoryService;

            _files = new ObservableCollection<FileViewModel>();

            // TODO: load directory from settings by key/number
            CurrentDirectory = "/home/";

            ReloadFiles();
        }

        private void ReloadFiles()
        {
            var directories = _directoryService.GetDirectories(CurrentDirectory);
            var files = _fileService.GetFiles(CurrentDirectory);

            var directoriesModels = directories
                .Select(d => new FileViewModel(d.Name, d.LastModifiedDateTime));
            var filesModels = files
                .Select(f => new FileViewModel(f.Name, f.LastModifiedDateTime));

            _files.Clear();
            _files.AddRange(directoriesModels.Concat(filesModels));
        }
    }
}