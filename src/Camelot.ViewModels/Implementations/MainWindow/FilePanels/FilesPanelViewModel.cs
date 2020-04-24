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
using Camelot.Services.Interfaces;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Comparers;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using DynamicData;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class FilesPanelViewModel : ViewModelBase, IFilesPanelViewModel
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly IFilesSelectionService _filesSelectionService;
        private readonly IFileSystemNodeViewModelFactory _fileSystemNodeViewModelFactory;
        private readonly IFileSystemWatchingService _fileSystemWatchingService;
        private readonly IApplicationDispatcher _applicationDispatcher;
        private readonly IFilesPanelStateService _filesPanelStateService;
        private readonly ITabViewModelFactory _tabViewModelFactory;
        private readonly IFileSizeFormatter _fileSizeFormatter;
        private readonly IClipboardOperationsService _clipboardOperationsService;

        private readonly ObservableCollection<IFileSystemNodeViewModel> _fileSystemNodes;
        private readonly ObservableCollection<IFileSystemNodeViewModel> _selectedFileSystemNodes;
        private readonly ObservableCollection<ITabViewModel> _tabs;

        private string _currentDirectory;
        private ITabViewModel _selectedTab;

        private IEnumerable<FileViewModel> SelectedFiles => _selectedFileSystemNodes.OfType<FileViewModel>();
        
        private IEnumerable<DirectoryViewModel> SelectedDirectories => _selectedFileSystemNodes.OfType<DirectoryViewModel>();

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
        
        public IEnumerable<IFileSystemNodeViewModel> FileSystemNodes => _fileSystemNodes;

        public IList<IFileSystemNodeViewModel> SelectedFileSystemNodes => _selectedFileSystemNodes;
        
        public int SelectedFilesCount => SelectedFiles.Count();
        
        public int SelectedDirectoriesCount => SelectedDirectories.Count();

        public string SelectedFilesSize => _fileSizeFormatter.GetFormattedSize(SelectedFiles.Sum(f => f.Size));

        public IEnumerable<ITabViewModel> Tabs => _tabs;

        public ITabViewModel SelectedTab
        {
            get => _selectedTab;
            set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
        }
        
        public event EventHandler<EventArgs> ActivatedEvent;

        public event EventHandler<EventArgs> DeactivatedEvent;

        public ICommand ActivateCommand { get; }

        public ICommand RefreshCommand { get; }
        
        public ICommand SortFilesCommand { get; }
        
        public ICommand CopyToClipboardCommand { get; }

        public ICommand PasteFromClipboardCommand { get; }

        public FilesPanelViewModel(
            IFileService fileService,
            IDirectoryService directoryService,
            IFilesSelectionService filesSelectionService,
            IFileSystemNodeViewModelFactory fileSystemNodeViewModelFactory,
            IFileSystemWatchingService fileSystemWatchingService,
            IApplicationDispatcher applicationDispatcher,
            IFilesPanelStateService filesPanelStateService,
            ITabViewModelFactory tabViewModelFactory,
            IFileSizeFormatter fileSizeFormatter,
            IClipboardOperationsService clipboardOperationsService)
        {
            _fileService = fileService;
            _directoryService = directoryService;
            _filesSelectionService = filesSelectionService;
            _fileSystemNodeViewModelFactory = fileSystemNodeViewModelFactory;
            _fileSystemWatchingService = fileSystemWatchingService;
            _applicationDispatcher = applicationDispatcher;
            _filesPanelStateService = filesPanelStateService;
            _tabViewModelFactory = tabViewModelFactory;
            _fileSizeFormatter = fileSizeFormatter;
            _clipboardOperationsService = clipboardOperationsService;

            _fileSystemNodes = new ObservableCollection<IFileSystemNodeViewModel>();
            _selectedFileSystemNodes = new ObservableCollection<IFileSystemNodeViewModel>();

            ActivateCommand = ReactiveCommand.Create(Activate);
            RefreshCommand = ReactiveCommand.Create(ReloadFiles);
            SortFilesCommand = ReactiveCommand.Create<SortingColumn>(SortFiles);
            CopyToClipboardCommand = ReactiveCommand.CreateFromTask(CopyToClipboardAsync);
            PasteFromClipboardCommand = ReactiveCommand.CreateFromTask(PasteFromClipboardAsync);

            var state = _filesPanelStateService.GetPanelState();
            if (!state.Tabs.Any())
            {
                state.Tabs = GetDefaultTabs();
            }
            
            _tabs = new ObservableCollection<ITabViewModel>(state.Tabs.Select(Create));
            _tabs.CollectionChanged += TabsOnCollectionChanged;

            SelectTab(_tabs[state.SelectedTabIndex]);

            this.WhenAnyValue(x => x.CurrentDirectory, x => x.SelectedTab)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => SaveState());

            SubscribeToEvents();
        }

        public void Activate()
        {
            ActivatedEvent.Raise(this, EventArgs.Empty);

            SelectedTab.IsGloballyActive = true;
        }

        public void Deactivate()
        {
            var selectedFiles = SelectedFileSystemNodes.Select(f => f.FullPath).ToArray();
            SelectedFileSystemNodes.Clear();
            _filesSelectionService.UnselectFiles(selectedFiles);

            DeactivatedEvent.Raise(this, EventArgs.Empty);

            SelectedTab.IsGloballyActive = false;
        }

        public void CreateNewTab() => CreateNewTab(SelectedTab);

        public void CloseActiveTab() => CloseTab(SelectedTab);
        
        private void SortFiles(SortingColumn sortingColumn)
        {
            if (SelectedTab.SortingViewModel.SortingColumn == sortingColumn)
            {
                SelectedTab.SortingViewModel.ToggleSortingDirection();
            }
            else
            {
                SelectedTab.SortingViewModel.SortingColumn = sortingColumn;
            }
            
            ReloadFiles();
            SaveState();
        }

        private void TabsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => SaveState();

        private ITabViewModel Create(string directory)
        {
            var tabModel = new TabModel
            {
                Directory = directory,
                SortingSettings = GetSortingSettings(SelectedTab)
            };

            return Create(tabModel);
        }
        
        private ITabViewModel Create(TabModel tabModel)
        {
            var tabViewModel = _tabViewModelFactory.Create(tabModel);
            SubscribeToEvents(tabViewModel);

            return tabViewModel;
        }

        private void Remove(ITabViewModel tabViewModel)
        {
            UnsubscribeFromEvents(tabViewModel);

            _tabs.Remove(tabViewModel);
            
            if (SelectedTab == tabViewModel)
            {
                SelectTab(_tabs.First());
            }
        }

        private void SubscribeToEvents(ITabViewModel tabViewModel)
        {
            tabViewModel.ActivationRequested += TabViewModelOnActivationRequested;
            tabViewModel.NewTabRequested += TabViewModelOnNewTabRequested;
            tabViewModel.CloseRequested += TabViewModelOnCloseRequested;
            tabViewModel.ClosingTabsToTheLeftRequested += TabViewModelOnClosingTabsToTheLeftRequested;
            tabViewModel.ClosingTabsToTheRightRequested += TabViewModelOnClosingTabsToTheRightRequested;
            tabViewModel.ClosingAllTabsButThisRequested += TabViewModelOnClosingAllTabsButThisRequested;
        }

        private void UnsubscribeFromEvents(ITabViewModel tabViewModel)
        {
            tabViewModel.ActivationRequested -= TabViewModelOnActivationRequested;
            tabViewModel.NewTabRequested -= TabViewModelOnNewTabRequested;
            tabViewModel.CloseRequested -= TabViewModelOnCloseRequested;
            tabViewModel.ClosingTabsToTheLeftRequested -= TabViewModelOnClosingTabsToTheLeftRequested;
            tabViewModel.ClosingTabsToTheRightRequested -= TabViewModelOnClosingTabsToTheRightRequested;
            tabViewModel.ClosingAllTabsButThisRequested -= TabViewModelOnClosingAllTabsButThisRequested;
        }

        private void TabViewModelOnActivationRequested(object sender, EventArgs e)
        {
            var tabViewModel = (ITabViewModel) sender;

            SelectTab(tabViewModel);
        }
        
        private void TabViewModelOnNewTabRequested(object sender, EventArgs e)
        {
            var tabViewModel = (ITabViewModel) sender;
            
            CreateNewTab(tabViewModel);
        }

        private void TabViewModelOnCloseRequested(object sender, EventArgs e)
        {
            var tabViewModel = (ITabViewModel) sender;
            
            CloseTab(tabViewModel);
        }

        private void TabViewModelOnClosingTabsToTheLeftRequested(object sender, EventArgs e)
        {
            var tabViewModel = (ITabViewModel) sender;
            var tabPosition = _tabs.IndexOf(tabViewModel);
            var tabsToClose = _tabs.Take(tabPosition).ToArray();

            tabsToClose.ForEach(Remove);
        }
        
        private void TabViewModelOnClosingTabsToTheRightRequested(object sender, EventArgs e)
        {
            var tabViewModel = (ITabViewModel) sender;
            var tabPosition = _tabs.IndexOf(tabViewModel);
            var tabsToClose = _tabs.Skip(tabPosition + 1).ToArray();

            tabsToClose.ForEach(Remove);
        }

        private void TabViewModelOnClosingAllTabsButThisRequested(object sender, EventArgs e)
        {
            var tabViewModel = (ITabViewModel) sender;
            var tabsToClose = _tabs.Where(t => t != tabViewModel).ToArray();

            tabsToClose.ForEach(Remove);
        }
        
        private void CreateNewTab(ITabViewModel tabViewModel)
        {
            var tabPosition = _tabs.IndexOf(tabViewModel);
            var newTabViewModel = Create(tabViewModel.CurrentDirectory);
            
            _tabs.Insert(tabPosition, newTabViewModel);
        }
        
        private void CloseTab(ITabViewModel tabViewModel)
        {
            if (_tabs.Count > 1)
            {
                Remove(tabViewModel);
            }
        }
        
        private void SelectTab(ITabViewModel tabViewModel)
        {
            if (SelectedTab != null)
            {
                SelectedTab.IsActive = SelectedTab.IsGloballyActive = false;
            }

            tabViewModel.IsActive = tabViewModel.IsGloballyActive = true;
            SelectedTab = tabViewModel;
            if (CurrentDirectory == tabViewModel.CurrentDirectory)
            {
                ReloadFiles();
            }
            else
            {
                CurrentDirectory = tabViewModel.CurrentDirectory;
            }
            
            Activate();
        }

        private void SubscribeToEvents()
        {
            _selectedFileSystemNodes.CollectionChanged += SelectedFileSystemNodesOnCollectionChanged;

            void ReloadInUiThread() => _applicationDispatcher.Dispatch(ReloadFiles);

            // TODO: don't reload all files, process update properly
            _fileSystemWatchingService.FileCreated += (sender, args) => ReloadInUiThread();
            _fileSystemWatchingService.FileChanged += (sender, args) => ReloadInUiThread();
            _fileSystemWatchingService.FileRenamed += (sender, args) => ReloadInUiThread();
            _fileSystemWatchingService.FileDeleted += (sender, args) => ReloadInUiThread();
        }

        private void ReloadFiles()
        {
            if (!_directoryService.CheckIfExists(CurrentDirectory))
            {
                return;
            }

            _fileSystemWatchingService.StopWatching();

            var parentDirectory = _directoryService.GetParentDirectory(CurrentDirectory);
            var directories = _directoryService.GetDirectories(CurrentDirectory);
            var files = _fileService.GetFiles(CurrentDirectory);

            var sortingViewModel = SelectedTab.SortingViewModel;
            var directoriesComparer = new DirectoryModelsFileSystemNodesComparer(
                sortingViewModel.IsSortingByAscendingEnabled, sortingViewModel.SortingColumn);
            var directoriesViewModels = directories
                .OrderBy(d => d, directoriesComparer)
                .Select(d => _fileSystemNodeViewModelFactory.Create(d));
            
            var filesComparer = new FileModelsFileSystemNodesComparer(
                sortingViewModel.IsSortingByAscendingEnabled, sortingViewModel.SortingColumn);
            var filesViewModels = files
                .OrderBy(f => f, filesComparer)
                .Select(_fileSystemNodeViewModelFactory.Create);
            
            var models = directoriesViewModels.Concat(filesViewModels);

            _fileSystemNodes.Clear();
            _fileSystemNodes.AddRange(models);

            if (parentDirectory != null)
            {
                var parentDirectoryViewModel = _fileSystemNodeViewModelFactory.Create(parentDirectory, true);
                
                _fileSystemNodes.Insert(0, parentDirectoryViewModel);
            }

            _fileSystemWatchingService.StartWatching(CurrentDirectory);
        }

        private void SelectedFileSystemNodesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var filesToAdd = e.NewItems?
                .Cast<IFileSystemNodeViewModel>()
                .Select(f => f.FullPath);
            if (filesToAdd != null)
            {
                _filesSelectionService.SelectFiles(filesToAdd);
            }

            var filesToRemove = e.OldItems?
                .Cast<IFileSystemNodeViewModel>()
                .Select(f => f.FullPath);
            if (filesToRemove != null)
            {
                _filesSelectionService.UnselectFiles(filesToRemove);
            }
            
            this.RaisePropertyChanged(nameof(SelectedFilesCount));
            this.RaisePropertyChanged(nameof(SelectedFilesSize));
            this.RaisePropertyChanged(nameof(SelectedDirectoriesCount));
        }

        private void SaveState()
        {
            Task.Factory.StartNew(() =>
            {
                var tabs = _tabs.Select(CreateFrom).ToList();
                var selectedTabIndex = _tabs.IndexOf(_selectedTab);
                var state = new PanelModel
                {
                    Tabs = tabs,
                    SelectedTabIndex = selectedTabIndex
                };

                _filesPanelStateService.SavePanelState(state);
            }, TaskCreationOptions.LongRunning);
        }

        private static TabModel CreateFrom(ITabViewModel tabViewModel) =>
            new TabModel {Directory = tabViewModel.CurrentDirectory, SortingSettings = GetSortingSettings(tabViewModel)};

        private static SortingSettings GetSortingSettings(ITabViewModel tabViewModel) =>
            new SortingSettings
            {
                IsAscending = tabViewModel.SortingViewModel.IsSortingByAscendingEnabled,
                SortingMode = (int) tabViewModel.SortingViewModel.SortingColumn
            };

        private Task CopyToClipboardAsync() =>
            _clipboardOperationsService.CopyFilesAsync(_filesSelectionService.SelectedFiles);

        private Task PasteFromClipboardAsync() =>
            _clipboardOperationsService.PasteFilesAsync(CurrentDirectory);
        
        private List<TabModel> GetDefaultTabs()
        {
            var rootDirectoryTab = new TabModel
            {
                Directory = _directoryService.GetAppRootDirectory()
            };

            return new List<TabModel> {rootDirectoryTab};
        }
    }
}