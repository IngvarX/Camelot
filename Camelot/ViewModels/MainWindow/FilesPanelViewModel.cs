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
        private readonly ObservableCollection<TabViewModel> _tabs;

        private string _currentDirectory;
        private TabViewModel _selectedTab;

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
                    SelectedTab.CurrentDirectory = _currentDirectory;
                }
            }
        }

        public IEnumerable<FileViewModel> Files => _files;

        public IList<FileViewModel> SelectedFiles => _selectedFiles;

        public IEnumerable<TabViewModel> Tabs => _tabs;

        public TabViewModel SelectedTab
        {
            get => _selectedTab;
            set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
        }

        public ICommand ActivateCommand { get; }

        public ICommand RefreshCommand { get; }

        public event EventHandler<EventArgs> ActivatedEvent;

        public event EventHandler<EventArgs> DeactivatedEvent;

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
            if (!state.Tabs.Any())
            {
                // TODO: get all roots
                state.Tabs = new List<string>
                {
                    _directoryService.GetAppRootDirectory()
                };
            }
            _tabs = new ObservableCollection<TabViewModel>(state.Tabs.Select(Create));

            SelectTab(_tabs[state.SelectedTabIndex]);

            this.WhenAnyValue(x => x.CurrentDirectory)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => SaveState());

            ReloadFiles();
            SubscribeToEvents();
        }

        public void Deactivate()
        {
            var selectedFiles = SelectedFiles.Select(f => f.FullPath).ToArray();
            SelectedFiles.Clear();
            _filesSelectionService.UnselectFiles(selectedFiles);

            DeactivatedEvent.Raise(this, EventArgs.Empty);
        }

        private TabViewModel Create(string directory)
        {
            var tabViewModel = new TabViewModel(directory);
            SubscribeToEvents(tabViewModel);

            return tabViewModel;
        }

        private void Remove(TabViewModel tabViewModel)
        {
            UnsubscribeFromEvents(tabViewModel);
            if (SelectedTab == tabViewModel)
            {
                SelectedTab = _tabs.First();
            }

            _tabs.Remove(tabViewModel);
        }

        private void SubscribeToEvents(TabViewModel tabViewModel)
        {
            tabViewModel.ActivationRequested += TabViewModelOnActivationRequested;
            tabViewModel.CloseRequested += TabViewModelOnCloseRequested;
        }

        private void UnsubscribeFromEvents(TabViewModel tabViewModel)
        {
            tabViewModel.ActivationRequested -= TabViewModelOnActivationRequested;
            tabViewModel.CloseRequested -= TabViewModelOnCloseRequested;
        }

        private void TabViewModelOnCloseRequested(object sender, EventArgs e)
        {
            var tabViewModel = (TabViewModel) sender;

            Remove(tabViewModel);
        }

        private void TabViewModelOnActivationRequested(object sender, EventArgs e)
        {
            var tabViewModel = (TabViewModel) sender;

            SelectTab(tabViewModel);
        }

        private void SelectTab(TabViewModel tabViewModel)
        {
            if (SelectedTab != null)
            {
                SelectedTab.IsActive = false;
            }

            tabViewModel.IsActive = true;
            SelectedTab = tabViewModel;
            CurrentDirectory = tabViewModel.CurrentDirectory;
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
            if (!_directoryService.CheckIfDirectoryExists(CurrentDirectory))
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