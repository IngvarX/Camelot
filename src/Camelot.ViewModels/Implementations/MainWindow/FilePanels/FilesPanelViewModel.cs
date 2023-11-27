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
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.EventArgs;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.Operations;
using Camelot.ViewModels.Services.Interfaces;

using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels;

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
    private readonly IFileSystemNodeViewModelComparerFactory _comparerFactory;
    private readonly IRecursiveSearchService _recursiveSearchService;
    private readonly IFilePanelDirectoryObserver _filePanelDirectoryObserver;
    private readonly IPermissionsService _permissionsService;
    private readonly IDialogService _dialogService;

    private readonly ObservableCollection<IFileSystemNodeViewModel> _fileSystemNodes;
    private readonly ObservableCollection<IFileSystemNodeViewModel> _selectedFileSystemNodes;

    private CancellationTokenSource _cancellationTokenSource;
    private string _currentDirectory;
    private INodeSpecification _searchSpecification;
    private ISpecification<IFileSystemNodeViewModel> _filterSpecification;

    private IEnumerable<IFileViewModel> SelectedFiles => _selectedFileSystemNodes.OfType<IFileViewModel>();

    private IEnumerable<IDirectoryViewModel> SelectedDirectories => _selectedFileSystemNodes.OfType<IDirectoryViewModel>();

    private DirectoryModel ParentDirectory => CurrentDirectory is null
        ? null
        : _directoryService.GetParentDirectory(CurrentDirectory);

    public ITabViewModel SelectedTab => TabsListViewModel.SelectedTab;

    public ISearchViewModel SearchViewModel { get; }

    public IQuickSearchViewModel QuickSearchViewModel { get; }

    public ITabsListViewModel TabsListViewModel { get; }

    public IOperationsViewModel OperationsViewModel { get; }

    public IDirectorySelectorViewModel DirectorySelectorViewModel { get; }

    public IDragAndDropOperationsMediator DragAndDropOperationsMediator { get; }

    public IClipboardOperationsViewModel ClipboardOperationsViewModel { get; }

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

    public event EventHandler<SelectionAddedEventArgs> SelectionAdded;

    public event EventHandler<SelectionRemovedEventArgs> SelectionRemoved;

    public ICommand ActivateCommand { get; }

    public ICommand RefreshCommand { get; }

    public ICommand GoToParentDirectoryCommand { get; }

    public ICommand SortFilesCommand { get; }

    public FilesPanelViewModel(
        IFileService fileService,
        IDirectoryService directoryService,
        INodesSelectionService nodesSelectionService,
        INodeService nodeService,
        IFileSystemNodeViewModelFactory fileSystemNodeViewModelFactory,
        IFileSystemWatchingService fileSystemWatchingService,
        IApplicationDispatcher applicationDispatcher,
        IFileSizeFormatter fileSizeFormatter,
        IFileSystemNodeViewModelComparerFactory comparerFactory,
        IRecursiveSearchService recursiveSearchService,
        IFilePanelDirectoryObserver filePanelDirectoryObserver,
        IPermissionsService permissionsService,
        IDialogService dialogService,
        ISearchViewModel searchViewModel,
        IQuickSearchViewModel quickSearchViewModel,
        ITabsListViewModel tabsListViewModel,
        IOperationsViewModel operationsViewModel,
        IDirectorySelectorViewModel directorySelectorViewModel,
        IDragAndDropOperationsMediator dragAndDropOperationsMediator,
        IClipboardOperationsViewModel clipboardOperationsViewModel)
    {
        _fileService = fileService;
        _directoryService = directoryService;
        _nodesSelectionService = nodesSelectionService;
        _nodeService = nodeService;
        _fileSystemNodeViewModelFactory = fileSystemNodeViewModelFactory;
        _fileSystemWatchingService = fileSystemWatchingService;
        _applicationDispatcher = applicationDispatcher;
        _fileSizeFormatter = fileSizeFormatter;
        _comparerFactory = comparerFactory;
        _recursiveSearchService = recursiveSearchService;
        _filePanelDirectoryObserver = filePanelDirectoryObserver;
        _permissionsService = permissionsService;
        _dialogService = dialogService;

        SearchViewModel = searchViewModel;
        QuickSearchViewModel = quickSearchViewModel;
        TabsListViewModel = tabsListViewModel;
        OperationsViewModel = operationsViewModel;
        DirectorySelectorViewModel = directorySelectorViewModel;
        DragAndDropOperationsMediator = dragAndDropOperationsMediator;
        ClipboardOperationsViewModel = clipboardOperationsViewModel;

        _fileSystemNodes = new ObservableCollection<IFileSystemNodeViewModel>();
        _selectedFileSystemNodes = new ObservableCollection<IFileSystemNodeViewModel>();

        ActivateCommand = ReactiveCommand.Create(Activate);
        RefreshCommand = ReactiveCommand.Create(ReloadFiles);
        var canGoToParentDirectory = this.WhenAnyValue(vm => vm.ParentDirectory,
            (DirectoryModel dm) => dm is not null);
        GoToParentDirectoryCommand = ReactiveCommand.Create(GoToParentDirectory, canGoToParentDirectory);
        SortFilesCommand = ReactiveCommand.Create<SortingMode>(SortFiles);
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

    private void SortFiles(SortingMode sortingMode)
    {
        var sortingViewModel = SelectedTab.SortingViewModel;
        if (sortingViewModel.SortingColumn == sortingMode)
        {
            sortingViewModel.ToggleSortingDirection();
        }
        else
        {
            sortingViewModel.SortingColumn = sortingMode;
        }

        ReloadFiles();
    }

    private void SubscribeToEvents()
    {
        TabsListViewModel.SelectedTabChanged += TabsListViewModelOnSelectedTabChanged;
        SearchViewModel.SearchSettingsChanged += SearchViewModelOnSearchSettingsChanged;
        _filePanelDirectoryObserver.CurrentDirectoryChanged += async (_, _) => await UpdateStateAsync();
        _selectedFileSystemNodes.CollectionChanged += SelectedFileSystemNodesOnCollectionChanged;
        QuickSearchViewModel.QuickSearchFilterChanged += QuickSearchViewModelOnQuickSearchFilterChanged;

        _fileSystemWatchingService.NodeCreated += (_, args) =>
            ExecuteInUiThread(() => InsertNode(args.Node));
        _fileSystemWatchingService.NodeChanged += (_, args) =>
            ExecuteInUiThread(() => UpdateNode(args.Node));
        _fileSystemWatchingService.NodeRenamed += (_, args) =>
            ExecuteInUiThread(() => RecreateNode(args.Node, args.NewName));
        _fileSystemWatchingService.NodeDeleted += (_, args) =>
            ExecuteInUiThread(() => DeleteNode(args.Node));
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

        if (previousCurrentDirectory is not null)
        {
            _fileSystemWatchingService.StopWatching(previousCurrentDirectory);
        }

        ReloadFiles();
        _fileSystemWatchingService.StartWatching(CurrentDirectory);
        LoadDirectoryViewModel();

        CurrentDirectoryChanged.Raise(this, EventArgs.Empty);
        this.RaisePropertyChanged(nameof(ParentDirectory));

        QuickSearchViewModel.ClearQuickSearch();
    }

    private void UpdateNode(string nodePath) => RecreateNode(nodePath, nodePath);

    private void RecreateNode(string oldName, string newName)
    {
        var isSelected = CleanupSelection(oldName);
        RemoveNode(oldName);
        InsertNode(newName, isSelected);
    }

    private void InsertNode(string nodePath, bool isSelected = false)
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

        var index = GetInsertIndex(newNodeModel);
        _fileSystemNodes.Insert(index, newNodeModel);
        UpdateNodeFiltering(newNodeModel);

        if (isSelected)
        {
            SelectNode(nodePath);
        }
    }

    private void DeleteNode(string nodePath)
    {
        CleanupSelection(nodePath);
        RemoveNode(nodePath);
    }

    private void RemoveNode(string nodePath)
    {
        var nodeViewModel = GetViewModel(nodePath);
        if (nodeViewModel is not null)
        {
            _fileSystemNodes.Remove(nodeViewModel);
        }
    }

    private bool CleanupSelection(string nodePath)
    {
        var isSelected = CheckIfSelected(nodePath);
        if (isSelected)
        {
            UnselectNode(nodePath);
        }

        return isSelected;
    }

    private void SelectNode(string nodePath) =>
        SelectionAdded.Raise(this, new SelectionAddedEventArgs(nodePath));

    private void UnselectNode(string nodePath) =>
        SelectionRemoved.Raise(this, new SelectionRemovedEventArgs(nodePath));

    private bool CheckIfSelected(string nodePath) => _nodesSelectionService.SelectedNodes.Contains(nodePath);

    private void ReloadFiles()
    {
        CancelPreviousSearchIfNeeded();

        _searchSpecification = SearchViewModel.GetSpecification();
        if (_searchSpecification.IsRecursive)
        {
            RecursiveSearch(_searchSpecification);
        }
        else
        {
            Search(_searchSpecification);
        }

        InsertParentDirectory();
    }

    private void GoToParentDirectory() => _filePanelDirectoryObserver.CurrentDirectory = ParentDirectory.FullPath;

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
        if (ParentDirectory is not null)
        {
            var parentDirectoryViewModel = _fileSystemNodeViewModelFactory.Create(ParentDirectory, true);

            _fileSystemNodes.Insert(0, parentDirectoryViewModel);
        }
    }

    private void SelectedFileSystemNodesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        var nodesToAdd = e.NewItems?
            .Cast<IFileSystemNodeViewModel>()
            .Select(f => f.FullPath);
        if (nodesToAdd is not null)
        {
            _nodesSelectionService.SelectNodes(nodesToAdd);
        }

        var nodesToRemove = e.OldItems?
            .Cast<IFileSystemNodeViewModel>()
            .Select(f => f.FullPath);
        if (nodesToRemove is not null)
        {
            _nodesSelectionService.UnselectNodes(nodesToRemove);
        }

        this.RaisePropertyChanged(nameof(SelectedFilesCount));
        this.RaisePropertyChanged(nameof(SelectedFilesSize));
        this.RaisePropertyChanged(nameof(SelectedDirectoriesCount));
        this.RaisePropertyChanged(nameof(AreAnyFileSystemNodesSelected));
    }

    private void QuickSearchViewModelOnQuickSearchFilterChanged(object sender, QuickSearchFilterChangedEventArgs e)
    {
        _filterSpecification = QuickSearchViewModel.GetSpecification();
        _fileSystemNodes.ForEach(UpdateNodeFiltering);

        MoveSelection(e.Direction);
    }

    private void MoveSelection(SelectionChangeDirection direction)
    {
        switch (direction)
        {
            case SelectionChangeDirection.Backward:
                MoveSelection(-1);
                break;
            case SelectionChangeDirection.Forward:
                MoveSelection(1);
                break;
        }
    }

    private int GetInsertIndex(IFileSystemNodeViewModel newNodeViewModel)
    {
        var comparer = GetComparer();
        var index = _fileSystemNodes.BinarySearch(newNodeViewModel, comparer);

        return index < 0 ? index ^ -1 : index;
    }

    private IComparer<IFileSystemNodeViewModel> GetComparer() => _comparerFactory.Create(SelectedTab.SortingViewModel);

    private IFileSystemNodeViewModel GetViewModel(string nodePath) =>
        _fileSystemNodes.FirstOrDefault(n => n.FullPath == nodePath);

    private void ExecuteInUiThread(Action action) => _applicationDispatcher.Dispatch(action);

    private bool CheckIfShouldShowNode(string nodePath) =>
        _searchSpecification?.IsSatisfiedBy(_nodeService.GetNode(nodePath)) ?? true;

    private void MoveSelection(int step)
    {
        var (selectedIndex, selectedNode) = GetSelectedNodeWithIndex();
        var newNode = GetNewSelectedNode(selectedIndex, step);

        ChangeSelectedNode(selectedNode, newNode);
    }

    private IFileSystemNodeViewModel GetNewSelectedNode(int selectedIndex, int step)
    {
        var count = _fileSystemNodes.Count;

        var start = selectedIndex > -1 ? selectedIndex + step : 0;
        start = (start + count) % count;

        for (var i = start; i != start - step; i = (i + step + count) % count)
        {
            var file = _fileSystemNodes[i];
            if (!file.IsFilteredOut)
            {
                return file;
            }
        }

        return null;
    }

    private (int, IFileSystemNodeViewModel) GetSelectedNodeWithIndex()
    {
        // TODO: check if possible to improve
        var currentSelectedNode = _selectedFileSystemNodes.FirstOrDefault();

        return currentSelectedNode is null
            ? (-1, null)
            : (_fileSystemNodes.IndexOf(currentSelectedNode), currentSelectedNode);
    }

    private void ChangeSelectedNode(IFileSystemNodeViewModel oldNode, IFileSystemNodeViewModel newNode)
    {
        if (newNode is null)
        {
            return;
        }

        if (oldNode is not null)
        {
            UnselectNode(oldNode.FullPath);
        }

        SelectNode(newNode.FullPath);
    }

    private void UpdateNodeFiltering(IFileSystemNodeViewModel nodeViewModel) =>
        nodeViewModel.IsFilteredOut = CheckIfShouldFilterOutNode(nodeViewModel);

    private bool CheckIfShouldFilterOutNode(IFileSystemNodeViewModel nodeViewModel) =>
        !_filterSpecification?.IsSatisfiedBy(nodeViewModel) ?? false;
}
