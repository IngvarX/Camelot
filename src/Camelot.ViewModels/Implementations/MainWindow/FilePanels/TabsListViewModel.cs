using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class TabsListViewModel : ViewModelBase, ITabsListViewModel
    {
        private readonly IFilesPanelStateService _filesPanelStateService;
        private readonly IDirectoryService _directoryService;
        private readonly ITabViewModelFactory _tabViewModelFactory;
        private readonly IFilesOperationsMediator _filesOperationsMediator;
        private readonly IHomeDirectoryProvider _homeDirectoryProvider;
        private readonly ObservableCollection<ITabViewModel> _tabs;

        private ITabViewModel _selectedTab;

        public IEnumerable<ITabViewModel> Tabs => _tabs;

        public ITabViewModel SelectedTab
        {
            get => _selectedTab;
            private set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
        }

        public event EventHandler<EventArgs> SelectedTabChanged;

        public ICommand SelectTabToTheLeftCommand { get; }

        public ICommand SelectTabToTheRightCommand { get; }

        public TabsListViewModel(
            IFilesPanelStateService filesPanelStateService,
            IDirectoryService directoryService,
            ITabViewModelFactory tabViewModelFactory,
            IFilesOperationsMediator filesOperationsMediator,
            IHomeDirectoryProvider homeDirectoryProvider,
            FilePanelConfiguration filePanelConfiguration)
        {
            _filesPanelStateService = filesPanelStateService;
            _directoryService = directoryService;
            _tabViewModelFactory = tabViewModelFactory;
            _filesOperationsMediator = filesOperationsMediator;
            _homeDirectoryProvider = homeDirectoryProvider;

            _tabs = SetupTabs();

            SelectTabToTheLeftCommand = ReactiveCommand.Create(SelectTabToTheLeft);
            SelectTabToTheRightCommand = ReactiveCommand.Create(SelectTabToTheRight);

            this.WhenAnyValue(x => x.SelectedTab.CurrentDirectory, x => x.SelectedTab)
                .Throttle(TimeSpan.FromMilliseconds(filePanelConfiguration.SaveTimeoutMs))
                .Subscribe(_ => SaveState());
        }

        public void CreateNewTab(string directory = null, bool switchTo = false) =>
            CreateNewTab(SelectedTab, directory, switchTo);

        public void CloseActiveTab() => CloseTab(SelectedTab);

        public void SaveState() =>
            Task.Factory.StartNew(() =>
            {
                var tabs = _tabs.Select(CreateFrom).ToList();
                var selectedTabIndex = GetSelectedTabIndex();
                var state = new PanelStateModel
                {
                    Tabs = tabs,
                    SelectedTabIndex = selectedTabIndex
                };

                _filesPanelStateService.SavePanelState(state);
            }, TaskCreationOptions.LongRunning);


        private ObservableCollection<ITabViewModel> SetupTabs()
        {
            var state = _filesPanelStateService.GetPanelState();
            if (!state.Tabs.Any())
            {
                state.Tabs = GetDefaultTabs();
            }

            var tabs = GetInitialTabs(state.Tabs);
            tabs.CollectionChanged += TabsOnCollectionChanged;

            var index = state.SelectedTabIndex >= tabs.Count ? ^1 : state.SelectedTabIndex;
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

        private void SelectTab(ITabViewModel tabViewModel)
        {
            if (SelectedTab != null)
            {
                SelectedTab.IsActive = SelectedTab.IsGloballyActive = false;
            }

            tabViewModel.IsActive = tabViewModel.IsGloballyActive = true;
            SelectedTab = tabViewModel;

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
            var tabModel = new TabStateModel
            {
                Directory = directory,
                SortingSettings = GetSortingSettings(SelectedTab)
            };

            return CreateViewModelFrom(tabModel);
        }

        private ITabViewModel CreateViewModelFrom(TabStateModel tabModel)
        {
            var tabViewModel = _tabViewModelFactory.Create(tabModel);
            SubscribeToEvents(tabViewModel);

            return tabViewModel;
        }

        private void Remove(ITabViewModel tabViewModel)
        {
            UnsubscribeFromEvents(tabViewModel);

            var index = _tabs.IndexOf(tabViewModel);
            _tabs.Remove(tabViewModel);

            if (SelectedTab == tabViewModel)
            {
                var newActiveTabIndex = index > 0 ? index - 1 : 0;
                SelectTab(_tabs[newActiveTabIndex]);
            }
        }

        private void SubscribeToEvents(ITabViewModel tabViewModel)
        {
            tabViewModel.ActivationRequested += TabViewModelOnActivationRequested;
            tabViewModel.NewTabRequested += TabViewModelOnNewTabRequested;
            tabViewModel.NewTabOnOtherPanelRequested += TabViewModelOnNewTabOnOtherPanelRequested;
            tabViewModel.CloseRequested += TabViewModelOnCloseRequested;
            tabViewModel.ClosingTabsToTheLeftRequested += TabViewModelOnClosingTabsToTheLeftRequested;
            tabViewModel.ClosingTabsToTheRightRequested += TabViewModelOnClosingTabsToTheRightRequested;
            tabViewModel.ClosingAllTabsButThisRequested += TabViewModelOnClosingAllTabsButThisRequested;
        }

        private void UnsubscribeFromEvents(ITabViewModel tabViewModel)
        {
            tabViewModel.ActivationRequested -= TabViewModelOnActivationRequested;
            tabViewModel.NewTabRequested -= TabViewModelOnNewTabRequested;
            tabViewModel.NewTabOnOtherPanelRequested -= TabViewModelOnNewTabOnOtherPanelRequested;
            tabViewModel.CloseRequested -= TabViewModelOnCloseRequested;
            tabViewModel.ClosingTabsToTheLeftRequested -= TabViewModelOnClosingTabsToTheLeftRequested;
            tabViewModel.ClosingTabsToTheRightRequested -= TabViewModelOnClosingTabsToTheRightRequested;
            tabViewModel.ClosingAllTabsButThisRequested -= TabViewModelOnClosingAllTabsButThisRequested;
        }

        private void TabViewModelOnActivationRequested(object sender, EventArgs e) => SelectTab((ITabViewModel) sender);

        private void TabViewModelOnNewTabRequested(object sender, EventArgs e) => CreateNewTab((ITabViewModel) sender);

        private void TabViewModelOnNewTabOnOtherPanelRequested(object sender, EventArgs e) =>
            CreateNewTabOnOtherPanel((ITabViewModel) sender);

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

        private void TabsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => SaveState();

        private void TabViewModelOnClosingAllTabsButThisRequested(object sender, EventArgs e)
        {
            var tabViewModel = (ITabViewModel) sender;
            var tabsToClose = _tabs.Where(t => t != tabViewModel).ToArray();

            tabsToClose.ForEach(Remove);
        }

        private void CreateNewTab(ITabViewModel tabViewModel, string directory = null, bool switchToTab = true)
        {
            var insertIndex = _tabs.IndexOf(tabViewModel) + 1;
            var newTabViewModel = CreateViewModelFrom(directory ?? tabViewModel.CurrentDirectory);

            _tabs.Insert(insertIndex, newTabViewModel);
            if (switchToTab)
            {
                SelectTab(newTabViewModel);
            }
        }

        private void CreateNewTabOnOtherPanel(ITabViewModel tabViewModel)
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

        private static TabStateModel CreateFrom(ITabViewModel tabViewModel) =>
            new TabStateModel
            {
                Directory = tabViewModel.CurrentDirectory,
                SortingSettings = GetSortingSettings(tabViewModel)
            };

        private static SortingSettingsStateModel GetSortingSettings(ITabViewModel tabViewModel) =>
            new SortingSettingsStateModel
            {
                IsAscending = tabViewModel.SortingViewModel.IsSortingByAscendingEnabled,
                SortingMode = tabViewModel.SortingViewModel.SortingColumn
            };

        private int GetSelectedTabIndex() => _tabs.IndexOf(SelectedTab);
    }
}