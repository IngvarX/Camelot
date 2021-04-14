using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Avalonia.Interfaces;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.RecursiveSearch;
using Camelot.Services.Abstractions.Specifications;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.Operations;
using Camelot.ViewModels.Services.Interfaces;
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
        private readonly IRecursiveSearchService _recursiveSearchService;
        private readonly IFilePanelDirectoryObserver _filePanelDirectoryObserver;

        private readonly ObservableCollection<IFileSystemNodeViewModel> _fileSystemNodes;
        private readonly ObservableCollection<IFileSystemNodeViewModel> _selectedFileSystemNodes;

        private CancellationTokenSource _cancellationTokenSource;
        private string _currentDirectory;

        private IEnumerable<IFileViewModel> SelectedFiles => _selectedFileSystemNodes.OfType<IFileViewModel>();

        private IEnumerable<IDirectoryViewModel> SelectedDirectories => _selectedFileSystemNodes.OfType<IDirectoryViewModel>();

        public ITabViewModel SelectedTab => TabsListViewModel.SelectedTab;

        public ISearchViewModel SearchViewModel { get; }

        public ITabsListViewModel TabsListViewModel { get; }

        public IOperationsViewModel OperationsViewModel { get; }

        public IDirectorySelectorViewModel DirectorySelectorViewModel { get; }

        public IDragAndDropOperationsMediator DragAndDropOperationsMediator { get; }

        public bool IsActive => SelectedTab.IsGloballyActive;

        public IEnumerable<IFileSystemNodeViewModel> FileSystemNodes => _fileSystemNodes;

        public IList<IFileSystemNodeViewModel> SelectedFileSystemNodes => _selectedFileSystemNodes;

        public bool AreAnyFileSystemNodesSelected => _selectedFileSystemNodes.Any();

        public int SelectedFilesCount => SelectedFiles.Count();

        public int SelectedDirectoriesCount => SelectedDirectories.Count();

        public string SelectedFilesSize => _fileSizeFormatter.GetFormattedSize(SelectedFiles.Sum(f => f.Size));

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set => _filePanelDirectoryObserver.CurrentDirectory = value;
        }

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
            IRecursiveSearchService recursiveSearchService,
            IFilePanelDirectoryObserver filePanelDirectoryObserver,
            ISearchViewModel searchViewModel,
            ITabsListViewModel tabsListViewModel,
            IOperationsViewModel operationsViewModel,
            IDirectorySelectorViewModel directorySelectorViewModel,
            IDragAndDropOperationsMediator dragAndDropOperationsMediator)
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
            _recursiveSearchService = recursiveSearchService;
            _filePanelDirectoryObserver = filePanelDirectoryObserver;

            SearchViewModel = searchViewModel;
            TabsListViewModel = tabsListViewModel;
            OperationsViewModel = operationsViewModel;
            DirectorySelectorViewModel = directorySelectorViewModel;
            DragAndDropOperationsMediator = dragAndDropOperationsMediator;

            _fileSystemNodes = new ObservableCollection<IFileSystemNodeViewModel>();
            _selectedFileSystemNodes = new ObservableCollection<IFileSystemNodeViewModel>();

            ActivateCommand = ReactiveCommand.Create(Activate);
            RefreshCommand = ReactiveCommand.Create(ReloadFiles);
            SortFilesCommand = ReactiveCommand.Create<SortingMode>(SortFiles);
            CopyToClipboardCommand = ReactiveCommand.CreateFromTask(CopyToClipboardAsync);
            PasteFromClipboardCommand = ReactiveCommand.CreateFromTask(PasteFromClipboardAsync);

            SubscribeToEvents();
            UpdateState();
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
            _filePanelDirectoryObserver.CurrentDirectoryChanged += (sender, args) => UpdateState();
            _selectedFileSystemNodes.CollectionChanged += SelectedFileSystemNodesOnCollectionChanged;

            _fileSystemWatchingService.NodeCreated += (sender, args) =>
                ExecuteInUiThread(() => InsertNode(args.Node));
            _fileSystemWatchingService.NodeChanged += (sender, args) =>
                ExecuteInUiThread(() => UpdateNode(args.Node));
            _fileSystemWatchingService.NodeRenamed += (sender, args) =>
                ExecuteInUiThread(() => RenameNode(args.Node, args.NewName));
            _fileSystemWatchingService.NodeDeleted += (sender, args) =>
                ExecuteInUiThread(() => RemoveNode(args.Node));
        }

        private void TabsListViewModelOnSelectedTabChanged(object sender, EventArgs e) =>
            this.RaisePropertyChanged(nameof(SelectedTab));

        private void SearchViewModelOnSearchSettingsChanged(object sender, EventArgs e) => ReloadFiles();

        private void UpdateState()
        {
            Activate();

            var previousCurrentDirectory = _currentDirectory;
            this.RaiseAndSetIfChanged(ref _currentDirectory, _filePanelDirectoryObserver.CurrentDirectory);

            if (previousCurrentDirectory != null)
            {
                _fileSystemWatchingService.StopWatching(previousCurrentDirectory);
            }

            ReloadFiles();
            _fileSystemWatchingService.StartWatching(CurrentDirectory);

            CurrentDirectoryChanged.Raise(this, EventArgs.Empty);
        }

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
            CancelPreviousSearchIfNeeded();

            var specification = SearchViewModel.GetSpecification();
            if (specification.IsRecursive)
            {
                RecursiveSearch(specification);
            }
            else
            {
                Search(specification);
            }

            InsertParentDirectory();
        }

        private void CancelPreviousSearchIfNeeded() => _cancellationTokenSource?.Cancel();

        private void RecursiveSearch(ISpecification<NodeModelBase> specification)
        {
            _fileSystemNodes.Clear();

            void RecursiveSearchResultOnNodeFoundEvent(object sender, NodeFoundEventArgs args) =>
                ExecuteInUiThread(() => InsertNode(args.NodePath));

            _cancellationTokenSource = new CancellationTokenSource();
            var recursiveSearchResult = _recursiveSearchService.Search(CurrentDirectory, specification,
                _cancellationTokenSource.Token);
            recursiveSearchResult.NodeFoundEvent += RecursiveSearchResultOnNodeFoundEvent;

            recursiveSearchResult.Task.Value.ContinueWith(_ => recursiveSearchResult.NodeFoundEvent -= RecursiveSearchResultOnNodeFoundEvent);
        }

        private void Search(ISpecification<NodeModelBase> specification)
        {
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
        }

        private void InsertParentDirectory()
        {
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

        private Task PasteFromClipboardAsync() => _clipboardOperationsService.PasteFilesAsync(CurrentDirectory);

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

        private void ExecuteInUiThread(Action action) => _applicationDispatcher.Dispatch(action);
    }
}