using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Avalonia.Interfaces;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using DynamicData;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class FilesPanelViewModel : ViewModelBase, IFilesPanelViewModel
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly INodesSelectionService _nodesSelectionService;
        private readonly IFileSystemNodeViewModelFactory _fileSystemNodeViewModelFactory;
        private readonly IFileSystemWatchingService _fileSystemWatchingService;
        private readonly IApplicationDispatcher _applicationDispatcher;
        private readonly IFileSizeFormatter _fileSizeFormatter;
        private readonly IClipboardOperationsService _clipboardOperationsService;
        private readonly IFileSystemNodeViewModelComparerFactory _comparerFactory;

        private readonly ObservableCollection<IFileSystemNodeViewModel> _fileSystemNodes;
        private readonly ObservableCollection<IFileSystemNodeViewModel> _selectedFileSystemNodes;
        private readonly object _locker;

        private string _currentDirectory;

        private IEnumerable<IFileViewModel> SelectedFiles => _selectedFileSystemNodes.OfType<IFileViewModel>();

        private IEnumerable<IDirectoryViewModel> SelectedDirectories => _selectedFileSystemNodes.OfType<IDirectoryViewModel>();

        public ITabViewModel SelectedTab => TabsListViewModel.SelectedTab;

        public ISearchViewModel SearchViewModel { get; }

        public ITabsListViewModel TabsListViewModel { get; }

        public IOperationsViewModel OperationsViewModel { get; }

        public bool IsActive => SelectedTab.IsGloballyActive;

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

        public event EventHandler<EventArgs> Activated;

        public event EventHandler<EventArgs> Deactivated;

        public event EventHandler<EventArgs> CurrentDirectoryChanged;

        public ICommand ActivateCommand { get; }

        public ICommand RefreshCommand { get; }

        public ICommand SortFilesCommand { get; }

        public ICommand CopyToClipboardCommand { get; }

        public ICommand PasteFromClipboardCommand { get; }

        public FilesPanelViewModel(
            IFileService fileService,
            IDirectoryService directoryService,
            INodesSelectionService nodesSelectionService,
            IFileSystemNodeViewModelFactory fileSystemNodeViewModelFactory,
            IFileSystemWatchingService fileSystemWatchingService,
            IApplicationDispatcher applicationDispatcher,
            IFileSizeFormatter fileSizeFormatter,
            IClipboardOperationsService clipboardOperationsService,
            IFileSystemNodeViewModelComparerFactory comparerFactory,
            ISearchViewModel searchViewModel,
            ITabsListViewModel tabsListViewModel,
            IOperationsViewModel operationsViewModel)
        {
            _fileService = fileService;
            _directoryService = directoryService;
            _nodesSelectionService = nodesSelectionService;
            _fileSystemNodeViewModelFactory = fileSystemNodeViewModelFactory;
            _fileSystemWatchingService = fileSystemWatchingService;
            _applicationDispatcher = applicationDispatcher;
            _fileSizeFormatter = fileSizeFormatter;
            _clipboardOperationsService = clipboardOperationsService;
            _comparerFactory = comparerFactory;

            SearchViewModel = searchViewModel;
            TabsListViewModel = tabsListViewModel;
            OperationsViewModel = operationsViewModel;

            _fileSystemNodes = new ObservableCollection<IFileSystemNodeViewModel>();
            _selectedFileSystemNodes = new ObservableCollection<IFileSystemNodeViewModel>();
            _locker = new object();

            ActivateCommand = ReactiveCommand.Create(Activate);
            RefreshCommand = ReactiveCommand.Create(ReloadFiles);
            SortFilesCommand = ReactiveCommand.Create<SortingMode>(SortFiles);
            CopyToClipboardCommand = ReactiveCommand.CreateFromTask(CopyToClipboardAsync);
            PasteFromClipboardCommand = ReactiveCommand.CreateFromTask(PasteFromClipboardAsync);

            SubscribeToEvents();
            CurrentDirectory = SelectedTab.CurrentDirectory;
        }

        public void Activate()
        {
            Activated.Raise(this, EventArgs.Empty);

            SelectedTab.IsGloballyActive = true;
        }

        public void Deactivate()
        {
            var selectedFiles = SelectedFileSystemNodes.Select(f => f.FullPath).ToArray();
            SelectedFileSystemNodes.Clear();
            _nodesSelectionService.UnselectNodes(selectedFiles);

            Deactivated.Raise(this, EventArgs.Empty);

            SelectedTab.IsGloballyActive = false;
        }

        public void OpenLastSelectedFile()
        {
            var lastSelected = _selectedFileSystemNodes.LastOrDefault();

            lastSelected?.OpenCommand.Execute(null);
        }

        private void SortFiles(SortingMode sortingMode)
        {
            if (SelectedTab.SortingViewModel.SortingColumn == sortingMode)
            {
                SelectedTab.SortingViewModel.ToggleSortingDirection();
            }
            else
            {
                SelectedTab.SortingViewModel.SortingColumn = sortingMode;
            }

            ReloadFiles();
            TabsListViewModel.SaveState();
        }

        private void SubscribeToEvents()
        {
            TabsListViewModel.SelectedTabChanged += TabsListViewModelOnSelectedTabChanged;
            SearchViewModel.SearchSettingsChanged += SearchViewModelOnSearchSettingsChanged;
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

        private void TabsListViewModelOnSelectedTabChanged(object sender, EventArgs e)
        {
            CurrentDirectory = SelectedTab.CurrentDirectory;
            this.RaisePropertyChanged(nameof(SelectedTab));
        }

        private void SearchViewModelOnSearchSettingsChanged(object sender, EventArgs e) =>
            _applicationDispatcher.Dispatch(ReloadFiles);

        private void RenameNode(string oldName, string newName)
        {
            RemoveNode(oldName);
            InsertNode(newName);
        }

        private void InsertNode(string nodePath)
        {
            var newNodeModel = _fileSystemNodeViewModelFactory.Create(nodePath);
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

            var specification = SearchViewModel.GetSpecification();
            var directories = _directoryService.GetChildDirectories(CurrentDirectory, specification);
            var files = _fileService.GetFiles(CurrentDirectory, specification);

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
            var nodesToAdd = e.NewItems?
                .Cast<IFileSystemNodeViewModel>()
                .Select(f => f.FullPath);
            if (nodesToAdd != null)
            {
                _nodesSelectionService.SelectNodes(nodesToAdd);
            }

            var nodesToRemove = e.OldItems?
                .Cast<IFileSystemNodeViewModel>()
                .Select(f => f.FullPath);
            if (nodesToRemove != null)
            {
                _nodesSelectionService.UnselectNodes(nodesToRemove);
            }

            this.RaisePropertyChanged(nameof(SelectedFilesCount));
            this.RaisePropertyChanged(nameof(SelectedFilesSize));
            this.RaisePropertyChanged(nameof(SelectedDirectoriesCount));
            this.RaisePropertyChanged(nameof(AreAnyFileSystemNodesSelected));
        }

        private Task CopyToClipboardAsync() =>
            _clipboardOperationsService.CopyFilesAsync(_nodesSelectionService.SelectedNodes);

        private Task PasteFromClipboardAsync() =>
            _clipboardOperationsService.PasteFilesAsync(CurrentDirectory);

        private int GetInsertIndex(IFileSystemNodeViewModel newNodeViewModel)
        {
            var comparer = GetComparer();
            var index = _fileSystemNodes.BinarySearch(newNodeViewModel, comparer);

            return index < 0 ? index ^ -1 : index;
        }

        private IComparer<IFileSystemNodeViewModel> GetComparer() =>
            _comparerFactory.Create(SelectedTab.SortingViewModel);

        private IFileSystemNodeViewModel GetViewModel(string nodePath) =>
            _fileSystemNodes.FirstOrDefault(n => n.FullPath == nodePath);
    }
}