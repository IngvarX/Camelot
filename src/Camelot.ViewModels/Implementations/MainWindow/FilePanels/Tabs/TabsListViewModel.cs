using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Collections;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Tabs;

public class TabsListViewModel : ViewModelBase, ITabsListViewModel
{
    private readonly IFilesPanelStateService _filesPanelStateService;
    private readonly IDirectoryService _directoryService;
    private readonly ITabViewModelFactory _tabViewModelFactory;
    private readonly IFilesOperationsMediator _filesOperationsMediator;
    private readonly IHomeDirectoryProvider _homeDirectoryProvider;
    private readonly IFilePanelDirectoryObserver _filePanelDirectoryObserver;

    private readonly ObservableCollection<ITabViewModel> _tabs;
    private readonly LimitedSizeStack<(ITabViewModel ViewModel, int Index)> _closedTabs;

    public IReadOnlyList<ITabViewModel> Tabs => _tabs;

    [Reactive]
    public ITabViewModel SelectedTab { get; private set; }

    public event EventHandler<EventArgs> SelectedTabChanged;

    public ICommand SelectTabToTheLeftCommand { get; }

    public ICommand SelectTabToTheRightCommand { get; }

    public ICommand ReopenClosedTabCommand { get; }

    public ICommand CreateNewTabCommand { get; }

    public ICommand CloseCurrentTabCommand { get; }

    public ICommand GoToTabCommand { get; }

    public ICommand GoToLastTabCommand { get; }

    public TabsListViewModel(
        IFilesPanelStateService filesPanelStateService,
        IDirectoryService directoryService,
        ITabViewModelFactory tabViewModelFactory,
        IFilesOperationsMediator filesOperationsMediator,
        IHomeDirectoryProvider homeDirectoryProvider,
        IFilePanelDirectoryObserver filePanelDirectoryObserver,
        TabsListConfiguration tabsListConfiguration)
    {
        _filesPanelStateService = filesPanelStateService;
        _directoryService = directoryService;
        _tabViewModelFactory = tabViewModelFactory;
        _filesOperationsMediator = filesOperationsMediator;
        _homeDirectoryProvider = homeDirectoryProvider;
        _filePanelDirectoryObserver = filePanelDirectoryObserver;

        _tabs = SetupTabs();
        _closedTabs = new LimitedSizeStack<(ITabViewModel ViewModel, int Index)>(tabsListConfiguration.SavedClosedTabsLimit);

        SelectTabToTheLeftCommand = ReactiveCommand.Create(SelectTabToTheLeft);
        SelectTabToTheRightCommand = ReactiveCommand.Create(SelectTabToTheRight);
        ReopenClosedTabCommand = ReactiveCommand.Create(ReopenClosedTab);
        CreateNewTabCommand = ReactiveCommand.Create(CreateCopyOfSelectedTab);
        CloseCurrentTabCommand = ReactiveCommand.Create(CloseActiveTab);
        GoToTabCommand = ReactiveCommand.Create<int>(SelectTab);
        GoToLastTabCommand = ReactiveCommand.Create(GoToLastTab);

        this.WhenAnyValue(x => x.SelectedTab.CurrentDirectory, x => x.SelectedTab)
            .Throttle(TimeSpan.FromMilliseconds(tabsListConfiguration.SaveTimeoutMs))
            .Subscribe(_ => SaveState());

        SubscribeToEvents();
    }

    public void CreateNewTab(string directory) => CreateNewBackgroundTab(directory);

    public void InsertBeforeTab(ITabViewModel tabViewModel, ITabViewModel tabViewModelToInsert)
    {
        var oppositePanelTargetTabIndex = GetTabIndex(tabViewModel);

        _tabs.Insert(oppositePanelTargetTabIndex, tabViewModelToInsert);
        SubscribeToEvents(tabViewModelToInsert);
    }

    private void SelectTab(int index)
    {
        if (index >= 0 && index < _tabs.Count)
        {
            SelectTab(_tabs[index]);
        }
    }

    private void CreateCopyOfSelectedTab() => CreateNewForegroundTab(SelectedTab);

    private void CloseActiveTab() => CloseTab(SelectedTab);

    private void GoToLastTab() => SelectTab(Tabs[^1]);

    private ObservableCollection<ITabViewModel> SetupTabs()
    {
        var state = _filesPanelStateService.GetPanelState();
        if (!state.Tabs.Any())
        {
            state.Tabs = GetDefaultTabs();
        }

        var tabs = GetInitialTabs(state.Tabs);
        tabs.CollectionChanged += TabsOnCollectionChanged;

        var index = state.SelectedTabIndex >= tabs.Count || state.SelectedTabIndex < 0
            ? ^1
            : state.SelectedTabIndex;
        SelectTab(tabs[index]);

        return tabs;
    }

    private void SelectTabToTheLeft()
    {
        var index = GetSelectedTabIndex();
        if (index > 0)
        {
            SelectTab(_tabs[index - 1]);
        }
    }

    private void SelectTabToTheRight()
    {
        var index = GetSelectedTabIndex();
        if (index < _tabs.Count - 1)
        {
            SelectTab(_tabs[index + 1]);
        }
    }

    private void ReopenClosedTab()
    {
        if (_closedTabs.IsEmpty)
        {
            return;
        }

        var (viewModel, index) = _closedTabs.Pop();

        SubscribeToEvents(viewModel);
        _tabs.Insert(index, viewModel);
        SelectTab(viewModel);
    }

    private void SelectTab(ITabViewModel tabViewModel)
    {
        if (SelectedTab is not null)
        {
            SelectedTab.IsActive = SelectedTab.IsGloballyActive = false;
        }

        tabViewModel.IsActive = tabViewModel.IsGloballyActive = true;
        SelectedTab = tabViewModel;

        _filePanelDirectoryObserver.CurrentDirectory = SelectedTab.CurrentDirectory;
        SelectedTabChanged.Raise(this, EventArgs.Empty);
    }

    private ObservableCollection<ITabViewModel> GetInitialTabs(IEnumerable<TabStateModel> tabModels)
    {
        var tabs = tabModels
            .Where(tm => _directoryService.CheckIfExists(tm.Directory))
            .ToList();
        if (!tabs.Any())
        {
            tabs = GetDefaultTabs();
        }

        return new ObservableCollection<ITabViewModel>(tabs.Select(CreateViewModelFrom));
    }

    private List<TabStateModel> GetDefaultTabs()
    {
        var rootDirectoryTab = new TabStateModel
        {
            Directory = _homeDirectoryProvider.HomeDirectoryPath,
            SortingSettings = new SortingSettingsStateModel
            {
                SortingMode = SortingMode.Date
            }
        };

        return new List<TabStateModel> {rootDirectoryTab};
    }

    private ITabViewModel CreateViewModelFrom(string directory)
    {
        var tabStateModel = new TabStateModel
        {
            Directory = directory,
            SortingSettings = SelectedTab.GetState().SortingSettings,
            History = new List<string> {directory}
        };

        return CreateViewModelFrom(tabStateModel);
    }

    private ITabViewModel CreateViewModelFrom(TabStateModel tabModel)
    {
        var tabViewModel = _tabViewModelFactory.Create(_filePanelDirectoryObserver, tabModel);
        SubscribeToEvents(tabViewModel);

        return tabViewModel;
    }

    private void Remove(ITabViewModel tabViewModel)
    {
        UnsubscribeFromEvents(tabViewModel);

        var index = GetTabIndex(tabViewModel);
        _tabs.RemoveAt(index);
        _closedTabs.Push((tabViewModel, index));

        if (SelectedTab == tabViewModel)
        {
            var newActiveTabIndex = index > 0 ? index - 1 : 0;
            SelectTab(_tabs[newActiveTabIndex]);
        }
    }

    private void SubscribeToEvents() =>
        _filePanelDirectoryObserver.CurrentDirectoryChanged += (_, _) =>
            SelectedTab.CurrentDirectory = _filePanelDirectoryObserver.CurrentDirectory;

    private void SubscribeToEvents(ITabViewModel tabViewModel)
    {
        tabViewModel.ActivationRequested += TabViewModelOnActivationRequested;
        tabViewModel.NewTabRequested += TabViewModelOnNewTabRequested;
        tabViewModel.NewTabOnOppositePanelRequested += TabViewModelOnNewTabOnOppositePanelRequested;
        tabViewModel.CloseRequested += TabViewModelOnCloseRequested;
        tabViewModel.ClosingTabsToTheLeftRequested += TabViewModelOnClosingTabsToTheLeftRequested;
        tabViewModel.ClosingTabsToTheRightRequested += TabViewModelOnClosingTabsToTheRightRequested;
        tabViewModel.ClosingAllTabsButThisRequested += TabViewModelOnClosingAllTabsButThisRequested;
        tabViewModel.MoveRequested += TabViewModelOnMoveRequested;
        tabViewModel.SortingViewModel.SortingSettingsChanged += SortingViewModelOnSortingSettingsChanged;
    }

    private void UnsubscribeFromEvents(ITabViewModel tabViewModel)
    {
        tabViewModel.ActivationRequested -= TabViewModelOnActivationRequested;
        tabViewModel.NewTabRequested -= TabViewModelOnNewTabRequested;
        tabViewModel.NewTabOnOppositePanelRequested -= TabViewModelOnNewTabOnOppositePanelRequested;
        tabViewModel.CloseRequested -= TabViewModelOnCloseRequested;
        tabViewModel.ClosingTabsToTheLeftRequested -= TabViewModelOnClosingTabsToTheLeftRequested;
        tabViewModel.ClosingTabsToTheRightRequested -= TabViewModelOnClosingTabsToTheRightRequested;
        tabViewModel.ClosingAllTabsButThisRequested -= TabViewModelOnClosingAllTabsButThisRequested;
        tabViewModel.MoveRequested -= TabViewModelOnMoveRequested;
        tabViewModel.SortingViewModel.SortingSettingsChanged -= SortingViewModelOnSortingSettingsChanged;
    }

    private void TabViewModelOnActivationRequested(object sender, EventArgs e) => SelectTab((ITabViewModel) sender);

    private void TabViewModelOnNewTabRequested(object sender, EventArgs e) => CreateNewForegroundTab((ITabViewModel) sender);

    private void TabViewModelOnNewTabOnOppositePanelRequested(object sender, EventArgs e) =>
        CreateNewTabOnOppositePanel((ITabViewModel) sender);

    private void TabViewModelOnCloseRequested(object sender, EventArgs e) => CloseTab((ITabViewModel) sender);

    private void TabViewModelOnClosingTabsToTheLeftRequested(object sender, EventArgs e)
    {
        var tabViewModel = (ITabViewModel) sender;
        var tabPosition = GetTabIndex(tabViewModel);
        var tabsToClose = _tabs.Take(tabPosition).ToArray();

        tabsToClose.ForEach(Remove);
    }

    private void TabViewModelOnClosingTabsToTheRightRequested(object sender, EventArgs e)
    {
        var tabViewModel = (ITabViewModel) sender;
        var tabPosition = GetTabIndex(tabViewModel);
        var tabsToClose = _tabs.Skip(tabPosition + 1).ToArray();

        tabsToClose.ForEach(Remove);
    }

    private void TabsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => SaveState();

    private void TabViewModelOnClosingAllTabsButThisRequested(object sender, EventArgs e)
    {
        var tabViewModel = (ITabViewModel) sender;
        var tabsToClose = _tabs.Where(t => t != tabViewModel).ToArray();

        tabsToClose.ForEach(Remove);
    }

    private void TabViewModelOnMoveRequested(object sender, TabMoveRequestedEventArgs e)
    {
        if (_tabs.Count == 1)
        {
            return;
        }

        var (sourceTab, targetTab) = ((ITabViewModel) sender, e.Target);
        var (sourceTabIndex, targetTabIndex) = (GetTabIndex(sourceTab), GetTabIndex(targetTab));

        if (targetTabIndex == -1) // drag n drop to opposite panel. remove tab from current panel and add to opposite
        {
            Remove(sourceTab);

            var oppositePanel = SelectedTab.IsGloballyActive
                ? _filesOperationsMediator.InactiveFilesPanelViewModel
                : _filesOperationsMediator.ActiveFilesPanelViewModel;

            oppositePanel.TabsListViewModel.InsertBeforeTab(targetTab, sourceTab);
        }
        else // same panel, just move tab to appropriate position
        {
            _tabs.RemoveAt(sourceTabIndex);
            _tabs.Insert(targetTabIndex, sourceTab);
        }
    }

    private void SortingViewModelOnSortingSettingsChanged(object sender, EventArgs e) => SaveState();

    private void CreateNewBackgroundTab(string directory)
    {
        var insertIndex = GetSelectedTabIndex() + 1;
        var newTabViewModel = CreateViewModelFrom(directory);

        _tabs.Insert(insertIndex, newTabViewModel);
    }

    private void CreateNewForegroundTab(ITabViewModel tabViewModel)
    {
        var insertIndex = GetTabIndex(tabViewModel) + 1;
        var newTabViewModel = CreateViewModelFrom(tabViewModel.CurrentDirectory);

        _tabs.Insert(insertIndex, newTabViewModel);
        SelectTab(newTabViewModel);
    }

    private void CreateNewTabOnOppositePanel(ITabViewModel tabViewModel)
    {
        var viewModel = SelectedTab.IsGloballyActive
            ? _filesOperationsMediator.InactiveFilesPanelViewModel
            : _filesOperationsMediator.ActiveFilesPanelViewModel;

        viewModel.TabsListViewModel.CreateNewTab(tabViewModel.CurrentDirectory);
    }

    private void CloseTab(ITabViewModel tabViewModel)
    {
        if (_tabs.Count > 1)
        {
            Remove(tabViewModel);
        }
    }

    private int GetSelectedTabIndex() => GetTabIndex(SelectedTab);

    private int GetTabIndex(ITabViewModel tabViewModel) => _tabs.IndexOf(tabViewModel);

    private void SaveState() =>
        Task.Run(() =>
        {
            var tabs = _tabs.Select(t => t.GetState()).ToList();
            var selectedTabIndex = GetSelectedTabIndex();
            var state = new PanelStateModel
            {
                Tabs = tabs,
                SelectedTabIndex = selectedTabIndex
            };

            _filesPanelStateService.SavePanelState(state);
        });
}