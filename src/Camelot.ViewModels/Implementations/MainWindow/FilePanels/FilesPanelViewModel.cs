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
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.Operations;
using Camelot.ViewModels.Services.Interfaces;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class FilesPanelViewModel : ViewModelBase, IFilesPanelViewModel
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly INodesSelectionService _nodesSelectionService;
        private readonly INodeService _nodeService;
        private readonly IFileSystemNodeViewModelFactory _fileSystemNodeViewModelFactory;
        private readonly IFileSystemWatchingService _fileSystemWatchingService;
        private readonly IApplicationDispatcher _applicationDispatcher;
        private readonly IFileSizeFormatter _fileSizeFormatter;
        private readonly IClipboardOperationsService _clipboardOperationsService;
        private readonly IFileSystemNodeViewModelComparerFactory _comparerFactory;
        private readonly IRecursiveSearchService _recursiveSearchService;
        private readonly IFilePanelDirectoryObserver _filePanelDirectoryObserver;
        private readonly IPermissionsService _permissionsService;
        private readonly IDialogService _dialogService;

        private readonly ObservableCollection<IFileSystemNodeViewModel> _fileSystemNodes;
        private readonly ObservableCollection<IFileSystemNodeViewModel> _selectedFileSystemNodes;

        private CancellationTokenSource _cancellationTokenSource;
        private string _currentDirectory;
        private INodeSpecification _specification;

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

        [Reactive]
        public IFileSystemNodeViewModel CurrentNode { get; private set; }

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
            INodeService nodeService,
            IFileSystemNodeViewModelFactory fileSystemNodeViewModelFactory,
            IFileSystemWatchingService fileSystemWatchingService,
            IApplicationDispatcher applicationDispatcher,
            IFileSizeFormatter fileSizeFormatter,
            IClipboardOperationsService clipboardOperationsService,
            IFileSystemNodeViewModelComparerFactory comparerFactory,
            IRecursiveSearchService recursiveSearchService,
            IFilePanelDirectoryObserver filePanelDirectoryObserver,
            IPermissionsService permissionsService,
            IDialogService dialogService,
            ISearchViewModel searchViewModel,
            ITabsListViewModel tabsListViewModel,
            IOperationsViewModel operationsViewModel,
            IDirectorySelectorViewModel directorySelectorViewModel,
            IDragAndDropOperationsMediator dragAndDropOperationsMediator)
        {
            _fileService = fileService;
            _directoryService = directoryService;
            _nodesSelectionService = nodesSelectionService;
            _nodeService = nodeService;
            _fileSystemNodeViewModelFactory = fileSystemNodeViewModelFactory;
            _fileSystemWatchingService = fileSystemWatchingService;
            _applicationDispatcher = applicationDispatcher;
            _fileSizeFormatter = fileSizeFormatter;
            _clipboardOperationsService = clipboardOperationsService;
            _comparerFactory = comparerFactory;
            _recursiveSearchService = recursiveSearchService;
            _filePanelDirectoryObserver = filePanelDirectoryObserver;
            _permissionsService = permissionsService;
            _dialogService = dialogService;

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
            UpdateStateAsync().Forget();
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

        public Task<bool> CanPasteAsync() => _clipboardOperationsService.CanPasteAsync();

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
        }

        private void SubscribeToEvents()
        {
            TabsListViewModel.SelectedTabChanged += TabsListViewModelOnSelectedTabChanged;
            SearchViewModel.SearchSettingsChanged += SearchViewModelOnSearchSettingsChanged;
            _filePanelDirectoryObserver.CurrentDirectoryChanged += async (sender, args) => await UpdateStateAsync();
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

        private void TabsListViewModelOnSelectedTabChanged(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(nameof(SelectedTab));
            Activate();
        }

        private void SearchViewModelOnSearchSettingsChanged(object sender, EventArgs e) => ReloadFiles();

        private async Task UpdateStateAsync()
        {
            Activate();

            var previousCurrentDirectory = _currentDirectory;
            var newCurrentDirectory = _filePanelDirectoryObserver.CurrentDirectory;

            if (!_permissionsService.CheckIfHasAccess(newCurrentDirectory))
            {
                var parameter = new AccessDeniedNavigationParameter(newCurrentDirectory);
                await _dialogService.ShowDialogAsync(nameof(AccessDeniedDialogViewModel), parameter);
                _filePanelDirectoryObserver.CurrentDirectory = previousCurrentDirectory;

                return;
            }

            this.RaiseAndSetIfChanged(ref _currentDirectory, newCurrentDirectory);

            if (previousCurrentDirectory != null)
            {
                _fileSystemWatchingService.StopWatching(previousCurrentDirectory);
            }

            ReloadFiles();
            _fileSystemWatchingService.StartWatching(CurrentDirectory);
            LoadDirectoryViewModel();

            CurrentDirectoryChanged.Raise(this, EventArgs.Empty);
        }

        private void RenameNode(string oldName, string newName)
        {
            RemoveNode(oldName);
            InsertNode(newName);
        }

        private void InsertNode(string nodePath)
        {
            if (!CheckIfShouldShowNode(nodePath))
            {
                return;
            }

            var newNodeModel = _fileSystemNodeViewModelFactory.Create(nodePath);
            if (newNodeModel is null)
            {
                return;
            }

            RemoveNode(nodePath);

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

            _specification = SearchViewModel.GetSpecification();
            if (_specification.IsRecursive)
            {
                RecursiveSearch(_specification);
            }
            else
            {
                Search(_specification);
            }

            InsertParentDirectory();
        }

        private void LoadDirectoryViewModel()
        {
            var directory = _directoryService.GetDirectory(CurrentDirectory);

            CurrentNode = _fileSystemNodeViewModelFactory.Create(directory);
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

        private bool CheckIfShouldShowNode(string nodePath) =>
            _specification is null || _specification.IsSatisfiedBy(_nodeService.GetNode(nodePath));
    }
}