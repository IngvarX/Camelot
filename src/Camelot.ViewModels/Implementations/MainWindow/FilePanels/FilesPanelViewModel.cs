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
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Interfaces;
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
        private readonly IFileSystemNodeViewModelComparerFactory _comparerFactory;

        private readonly ObservableCollection<IFileSystemNodeViewModel> _fileSystemNodes;
        private readonly ObservableCollection<IFileSystemNodeViewModel> _selectedFileSystemNodes;
        private readonly ObservableCollection<ITabViewModel> _tabs;
        private readonly object _locker;

        private string _currentDirectory;
        private ITabViewModel _selectedTab;

        private IEnumerable<IFileViewModel> SelectedFiles => _selectedFileSystemNodes.OfType<IFileViewModel>();

        private IEnumerable<IDirectoryViewModel> SelectedDirectories => _selectedFileSystemNodes.OfType<IDirectoryViewModel>();

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                Activate();

                if (!_directoryService.CheckIfExists(value))
                {
                    return;
                }

                var previousCurrentDirectory = _currentDirectory;
                this.RaiseAndSetIfChanged(ref _currentDirectory, value);

                if (previousCurrentDirectory != null)
                {
                    _fileSystemWatchingService.StopWatching(previousCurrentDirectory);
                }

                ReloadFiles();
                SelectedTab.CurrentDirectory = _currentDirectory;
                _fileSystemWatchingService.StartWatching(CurrentDirectory);

                CurrentDirectoryChanged.Raise(this, EventArgs.Empty);
            }
        }

        public IEnumerable<IFileSystemNodeViewModel> FileSystemNodes => _fileSystemNodes;

        public IList<IFileSystemNodeViewModel> SelectedFileSystemNodes => _selectedFileSystemNodes;

        public bool AreAnyFileSystemNodesSelected => _selectedFileSystemNodes.Any();

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

        public event EventHandler<EventArgs> CurrentDirectoryChanged;

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
            IClipboardOperationsService clipboardOperationsService,
            IFileSystemNodeViewModelComparerFactory comparerFactory,
            FilePanelConfiguration filePanelConfiguration)
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
            _comparerFactory = comparerFactory;

            _fileSystemNodes = new ObservableCollection<IFileSystemNodeViewModel>();
            _selectedFileSystemNodes = new ObservableCollection<IFileSystemNodeViewModel>();
            _locker = new object();

            ActivateCommand = ReactiveCommand.Create(Activate);
            RefreshCommand = ReactiveCommand.Create(ReloadFiles);
            SortFilesCommand = ReactiveCommand.Create<SortingColumn>(SortFiles);
            CopyToClipboardCommand = ReactiveCommand.CreateFromTask(CopyToClipboardAsync);
            PasteFromClipboardCommand = ReactiveCommand.CreateFromTask(PasteFromClipboardAsync);

            _tabs = SetupTabs();

            this.WhenAnyValue(x => x.CurrentDirectory, x => x.SelectedTab)
                .Throttle(TimeSpan.FromMilliseconds(filePanelConfiguration.SaveTimeoutMs))
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

        public void OpenLastSelectedFile()
        {
            var lastSelected = _selectedFileSystemNodes.Last();

            lastSelected.OpenCommand.Execute(null);
        }

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

        private void TabViewModelOnActivationRequested(object sender, EventArgs e) => SelectTab((ITabViewModel) sender);

        private void TabViewModelOnNewTabRequested(object sender, EventArgs e) => CreateNewTab((ITabViewModel) sender);

        private void TabViewModelOnCloseRequested(object sender, EventArgs e) => CloseTab((ITabViewModel) sender);

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

            CurrentDirectory = tabViewModel.CurrentDirectory;
        }

        private void SubscribeToEvents()
        {
            _selectedFileSystemNodes.CollectionChanged += SelectedFileSystemNodesOnCollectionChanged;

            void ExecuteInUiThread(Action action) => _applicationDispatcher.Dispatch(action);

            void Synchronize(Action action)
            {
                lock (_locker)
                {
                    action();
                }
            }

            void ExecuteSynchronized(Action action) => ExecuteInUiThread(() => Synchronize(action));

            _fileSystemWatchingService.NodeCreated += (sender, args) =>
                ExecuteSynchronized(() => InsertNode(args.Node));
            _fileSystemWatchingService.NodeChanged += (sender, args) =>
                ExecuteSynchronized(() => UpdateNode(args.Node));
            _fileSystemWatchingService.NodeRenamed += (sender, args) =>
                ExecuteSynchronized(() => RenameNode(args.Node, args.NewName));
            _fileSystemWatchingService.NodeDeleted += (sender, args) =>
                ExecuteSynchronized(() => RemoveNode(args.Node));
        }

        private void RenameNode(string oldName, string newName)
        {
            RemoveNode(oldName);
            InsertNode(newName);
        }

        private void InsertNode(string nodePath)
        {
            var newNodeModel = CreateFrom(nodePath);
            if (newNodeModel is null)
            {
                return;
            }

            var index = GetInsertIndex(newNodeModel);
            _fileSystemNodes.Insert(index, newNodeModel);
        }

        private void UpdateNode(string nodePath)
        {
            RemoveNode(nodePath);
            InsertNode(nodePath);
        }

        private void RemoveNode(string nodePath)
        {
            var nodeViewModel = GetViewModel(nodePath);
            if (nodeViewModel != null)
            {
                _fileSystemNodes.Remove(nodeViewModel);
            }
        }

        private void ReloadFiles()
        {
            if (!_directoryService.CheckIfExists(CurrentDirectory))
            {
                return;
            }

            var directories = _directoryService.GetChildDirectories(CurrentDirectory);
            var files = _fileService.GetFiles(CurrentDirectory);

            var directoriesViewModels = directories
                .Select(d => _fileSystemNodeViewModelFactory.Create(d));
            var filesViewModels = files
                .Select(_fileSystemNodeViewModelFactory.Create);

            var comparer = GetComparer();
            var models = directoriesViewModels
                .Concat(filesViewModels)
                .OrderBy(x => x, comparer);

            _fileSystemNodes.Clear();
            _fileSystemNodes.AddRange(models);

            var parentDirectory = _directoryService.GetParentDirectory(CurrentDirectory);
            if (parentDirectory != null)
            {
                var parentDirectoryViewModel = _fileSystemNodeViewModelFactory.Create(parentDirectory, true);

                _fileSystemNodes.Insert(0, parentDirectoryViewModel);
            }
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
            this.RaisePropertyChanged(nameof(AreAnyFileSystemNodesSelected));
        }

        private void SaveState() =>
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

        private static TabModel CreateFrom(ITabViewModel tabViewModel) =>
            new TabModel
            {
                Directory = tabViewModel.CurrentDirectory,
                SortingSettings = GetSortingSettings(tabViewModel)
            };

        private IFileSystemNodeViewModel CreateFrom(string path)
        {
            if (_fileService.CheckIfExists(path))
            {
                var fileModel = _fileService.GetFile(path);

                return _fileSystemNodeViewModelFactory.Create(fileModel);
            }

            if (_directoryService.CheckIfExists(path))
            {
                var directoryModel = _directoryService.GetDirectory(path);

                return _fileSystemNodeViewModelFactory.Create(directoryModel);
            }

            return null;
        }

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

        private ObservableCollection<ITabViewModel> SetupTabs()
        {
            var state = _filesPanelStateService.GetPanelState();
            if (!state.Tabs.Any())
            {
                state.Tabs = GetDefaultTabs();
            }

            var tabs = GetInitialTabs(state.Tabs);
            tabs.CollectionChanged += TabsOnCollectionChanged;

            SelectTab(tabs[state.SelectedTabIndex]);

            return tabs;
        }

        private int GetInsertIndex(IFileSystemNodeViewModel newNodeViewModel)
        {
            var comparer = GetComparer();
            var index = _fileSystemNodes.BinarySearch(newNodeViewModel, comparer);

            return index < 0 ? index ^ -1 : index;
        }

        private IComparer<IFileSystemNodeViewModel> GetComparer() =>
            _comparerFactory.Create(SelectedTab.SortingViewModel);

        private IFileSystemNodeViewModel GetViewModel(string nodePath) =>
            _fileSystemNodes.SingleOrDefault(n => n.FullPath == nodePath);

        private ObservableCollection<ITabViewModel> GetInitialTabs(IEnumerable<TabModel> tabModels)
        {
            var tabs = tabModels
                .Where(tm => _directoryService.CheckIfExists(tm.Directory))
                .Select(Create);

            return new ObservableCollection<ITabViewModel>(tabs);
        }
    }
}