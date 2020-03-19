using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ApplicationDispatcher.Interfaces;
using Camelot.DataAccess.Models;
using Camelot.Extensions;
using Camelot.Factories.Interfaces;
using Camelot.Services.Interfaces;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class FilesPanelViewModel : ViewModelBase
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly IFilesSelectionService _filesSelectionService;
        private readonly IFileViewModelFactory _fileViewModelFactory;
        private readonly IFileSystemWatchingService _fileSystemWatchingService;
        private readonly IApplicationDispatcher _applicationDispatcher;
        private readonly IFilesPanelStateService _filesPanelStateService;

        private readonly ObservableCollection<FileViewModel> _files;
        private readonly ObservableCollection<FileViewModel> _selectedFiles;

        private string _currentDirectory;

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                var hasChanged = _currentDirectory != value;
                this.RaiseAndSetIfChanged(ref _currentDirectory, value);

                if (hasChanged)
                {
                    ReloadFiles();
                    SaveState();
                }
            }
        }

        public IEnumerable<FileViewModel> Files => _files;

        public IList<FileViewModel> SelectedFiles => _selectedFiles;

        public ICommand ActivateCommand { get; }

        public ICommand RefreshCommand { get; }

        public event EventHandler<EventArgs> ActivatedEvent;

        public FilesPanelViewModel(
            IFileService fileService,
            IDirectoryService directoryService,
            IFilesSelectionService filesSelectionService,
            IFileViewModelFactory fileViewModelFactory,
            IFileSystemWatchingService fileSystemWatchingService,
            IApplicationDispatcher applicationDispatcher,
            IFilesPanelStateService filesPanelStateService)
        {
            _fileService = fileService;
            _directoryService = directoryService;
            _filesSelectionService = filesSelectionService;
            _fileViewModelFactory = fileViewModelFactory;
            _fileSystemWatchingService = fileSystemWatchingService;
            _applicationDispatcher = applicationDispatcher;
            _filesPanelStateService = filesPanelStateService;

            _files = new ObservableCollection<FileViewModel>();
            _selectedFiles = new ObservableCollection<FileViewModel>();

            ActivateCommand = ReactiveCommand.Create(Activate);
            RefreshCommand = ReactiveCommand.Create(ReloadFiles);

            var state = _filesPanelStateService.GetPanelState();
            // TODO: get root dir
            CurrentDirectory = state.Tabs.SingleOrDefault() ?? "/home/";

            this.WhenAnyValue(x => x.CurrentDirectory)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => SaveState());

            ReloadFiles();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _selectedFiles.CollectionChanged += SelectedFilesOnCollectionChanged;

            void ReloadInUiThread() => _applicationDispatcher.Dispatch(ReloadFiles);

            // TODO: don't reload all files, process update properly
            _fileSystemWatchingService.FileCreated += (sender, args) => ReloadInUiThread();
            _fileSystemWatchingService.FileChanged += (sender, args) => ReloadInUiThread();
            _fileSystemWatchingService.FileRenamed += (sender, args) => ReloadInUiThread();
            _fileSystemWatchingService.FileDeleted += (sender, args) => ReloadInUiThread();
        }

        private void Activate()
        {
            ActivatedEvent.Raise(this, EventArgs.Empty);
        }

        private void ReloadFiles()
        {
            if (!_directoryService.DirectoryExists(CurrentDirectory))
            {
                return;
            }

            _fileSystemWatchingService.StopWatching();

            var directories = _directoryService.GetDirectories(CurrentDirectory);
            var files = _fileService.GetFiles(CurrentDirectory);

            var directoriesModels = directories
                .Select(_fileViewModelFactory.Create);
            var filesModels = files
                .Select(_fileViewModelFactory.Create);
            var models = directoriesModels.Concat(filesModels).ToArray();

            _files.Clear();
            _files.AddRange(models);

            _fileSystemWatchingService.StartWatching(CurrentDirectory);
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
                _filesSelectionService.UnselectFiles(filesToRemove);
            }
        }

        private void SaveState()
        {
            Task.Run(() =>
            {
                var tabs = new List<string> {CurrentDirectory};
                var state = new PanelState {Tabs = tabs};

                _filesPanelStateService.SavePanelState(state);
            });
        }
    }
}