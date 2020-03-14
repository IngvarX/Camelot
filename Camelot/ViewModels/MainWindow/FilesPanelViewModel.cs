using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Camelot.Services.Interfaces;
using Camelot.Services.Models;
using DynamicData;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class FilesPanelViewModel : ViewModelBase
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly IFilesSelectionService _filesSelectionService;

        private readonly ObservableCollection<FileViewModel> _files;
        private readonly ObservableCollection<FileViewModel> _selectedFiles;

        private FileViewModel _selectedFile;
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

        public IList<FileViewModel> SelectedFiles => _selectedFiles;

        public FileViewModel SelectedFile
        {
            get => _selectedFile;
            set => this.RaiseAndSetIfChanged(ref _selectedFile, value);
        }

        public FilesPanelViewModel(
            IFileService fileService,
            IDirectoryService directoryService,
            IFilesSelectionService filesSelectionService)
        {
            _fileService = fileService;
            _directoryService = directoryService;
            _filesSelectionService = filesSelectionService;

            _files = new ObservableCollection<FileViewModel>();
            _selectedFiles = new ObservableCollection<FileViewModel>();
            _selectedFiles.CollectionChanged += SelectedFilesOnCollectionChanged;

            // TODO: load directory from settings by key/number
            CurrentDirectory = "/home/";

            ReloadFiles();
        }

        private void ReloadFiles()
        {
            var directories = _directoryService.GetDirectories(CurrentDirectory);
            var files = _fileService.GetFiles(CurrentDirectory);

            var directoriesModels = directories
                .Select(d => new FileViewModel(() => CurrentDirectory = d.FullPath, d.FullPath, d.LastModifiedDateTime));
            var filesModels = files
                .Select(f => new FileViewModel(null, f.FullPath, f.LastModifiedDateTime));

            _files.Clear();
            _files.AddRange(directoriesModels.Concat(filesModels));
        }

        private void SelectedFilesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var filesToAdd = e.NewItems?
                .Cast<FileViewModel>()
                .Select(f => f.FullPath);
            if (filesToAdd != null)
            {
                _filesSelectionService.SelectFiles(filesToAdd);
            }

            var filesToRemove = e.OldItems?
                .Cast<FileViewModel>()
                .Select(f => f.FullPath);
            if (filesToRemove != null)
            {
                _filesSelectionService.SelectFiles(filesToRemove);
            }
        }
    }
}