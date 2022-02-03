using System;
using System.Windows.Input;
using Camelot.ViewModels.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Interfaces.MainWindow.Operations;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;
using Camelot.ViewModels.Interfaces.Menu;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations;

public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    private readonly IFilesOperationsMediator _filesOperationsMediator;

    public ITabsListViewModel ActiveTabsListViewModel =>
        _filesOperationsMediator.ActiveFilesPanelViewModel.TabsListViewModel;

    public IDirectorySelectorViewModel ActiveDirectorySelectorViewModel =>
        _filesOperationsMediator.ActiveFilesPanelViewModel.DirectorySelectorViewModel;

    public IOperationsViewModel OperationsViewModel { get; }

    public IFilesPanelViewModel LeftFilesPanelViewModel { get; }

    public IFilesPanelViewModel RightFilesPanelViewModel { get; }

    public IMenuViewModel MenuViewModel { get; }

    public IOperationsStateViewModel OperationsStateViewModel { get; }

    public ITopOperationsViewModel TopOperationsViewModel { get; }

    public IDrivesListViewModel DrivesListViewModel { get; }

    public IFavouriteDirectoriesListViewModel FavouriteDirectoriesListViewModel { get; }

    public ICommand SearchCommand { get; }

    public ICommand SwitchPanelCommand { get; }

    public ICommand FocusDirectorySelectorCommand { get; }

    public MainWindowViewModel(
        IFilesOperationsMediator filesOperationsMediator,
        IOperationsViewModel operationsViewModel,
        IFilesPanelViewModel leftFilesPanelViewModel,
        IFilesPanelViewModel rightFilesPanelViewModel,
        IMenuViewModel menuViewModel,
        IOperationsStateViewModel operationsStateViewModel,
        ITopOperationsViewModel topOperationsViewModel,
        IDrivesListViewModel drivesListViewModel,
        IFavouriteDirectoriesListViewModel favouriteDirectoriesListViewModel)
    {
        _filesOperationsMediator = filesOperationsMediator;

        OperationsViewModel = operationsViewModel;
        LeftFilesPanelViewModel = leftFilesPanelViewModel;
        RightFilesPanelViewModel = rightFilesPanelViewModel;
        MenuViewModel = menuViewModel;
        OperationsStateViewModel = operationsStateViewModel;
        TopOperationsViewModel = topOperationsViewModel;
        DrivesListViewModel = drivesListViewModel;
        FavouriteDirectoriesListViewModel = favouriteDirectoriesListViewModel;

        SearchCommand = ReactiveCommand.Create(Search);
        SwitchPanelCommand = ReactiveCommand.Create(SwitchPanel);
        FocusDirectorySelectorCommand = ReactiveCommand.Create(FocusDirectorySelector);

        filesOperationsMediator.Register(leftFilesPanelViewModel, rightFilesPanelViewModel);
        filesOperationsMediator.ActiveFilesPanelChanged += FilesOperationsMediatorOnActiveFilesPanelChanged;
    }

    private void Search() => _filesOperationsMediator.ToggleSearchPanelVisibility();

    private void SwitchPanel() => _filesOperationsMediator.InactiveFilesPanelViewModel.Activate();

    private void FocusDirectorySelector() => ActiveDirectorySelectorViewModel.Activate();

    private void FilesOperationsMediatorOnActiveFilesPanelChanged(object sender, EventArgs e)
    {
        this.RaisePropertyChanged(nameof(ActiveTabsListViewModel));
        this.RaisePropertyChanged(nameof(ActiveDirectorySelectorViewModel));
    }
}