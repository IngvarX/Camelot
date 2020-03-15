using System;
using Camelot.Services.EventArgs;
using Camelot.Services.Interfaces;
using Camelot.ViewModels.MainWindow;
using ReactiveUI;

namespace Camelot.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IDirectoryService _directoryService;
        private FilesPanelViewModel _activeFilesPanelViewModel;

        public OperationsViewModel OperationsViewModel { get; }

        public FilesPanelViewModel LeftFilesPanelViewModel { get; }

        public FilesPanelViewModel RightFilesPanelViewModel { get; }

        public FilesPanelViewModel ActiveFilesPanelViewModel
        {
            get => _activeFilesPanelViewModel;
            set => this.RaiseAndSetIfChanged(ref _activeFilesPanelViewModel, value);
        }

        public MainWindowViewModel(
            IDirectoryService directoryService,
            OperationsViewModel operationsViewModel,
            FilesPanelViewModel leftFilesPanelViewModel,
            FilesPanelViewModel rightFilesPanelViewModel)
        {
            _directoryService = directoryService;
            OperationsViewModel = operationsViewModel;
            LeftFilesPanelViewModel = leftFilesPanelViewModel;
            // TODO: change
            ActiveFilesPanelViewModel = RightFilesPanelViewModel = rightFilesPanelViewModel;

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _directoryService.SelectedDirectoryChanged += DirectoryServiceOnSelectedDirectoryChanged;

            LeftFilesPanelViewModel.ActivatedEvent += FilesPanelViewModelOnActivatedEvent;
            RightFilesPanelViewModel.ActivatedEvent += FilesPanelViewModelOnActivatedEvent;
        }

        private void DirectoryServiceOnSelectedDirectoryChanged(object sender, SelectedDirectoryChangedEventArgs e)
        {
            ActiveFilesPanelViewModel.CurrentDirectory = e.NewDirectory;
        }

        private void FilesPanelViewModelOnActivatedEvent(object sender, EventArgs e)
        {
            ActiveFilesPanelViewModel = (FilesPanelViewModel) sender;
        }
    }
}
