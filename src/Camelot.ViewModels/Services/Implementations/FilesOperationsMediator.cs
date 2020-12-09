using System;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Services.Implementations
{
    public class FilesOperationsMediator : IFilesOperationsMediator
    {
        private readonly IDirectoryService _directoryService;

        public string OutputDirectory => InactiveFilesPanelViewModel.CurrentDirectory;

        public IFilesPanelViewModel ActiveFilesPanelViewModel { get; private set; }

        public IFilesPanelViewModel InactiveFilesPanelViewModel { get; private set; }

        public FilesOperationsMediator(
            IDirectoryService directoryService)
        {
            _directoryService = directoryService;

            SubscribeToEvents();
        }

        public void Register(IFilesPanelViewModel activeFilesPanelViewModel, IFilesPanelViewModel inactiveFilesPanelViewModel)
        {
            (ActiveFilesPanelViewModel, InactiveFilesPanelViewModel) = (activeFilesPanelViewModel, inactiveFilesPanelViewModel);

            SubscribeToEvents(ActiveFilesPanelViewModel);
            SubscribeToEvents(InactiveFilesPanelViewModel);

            UpdateCurrentDirectory();

            ActiveFilesPanelViewModel.Activate();
            InactiveFilesPanelViewModel.Deactivate();
        }

        public void ToggleSearchPanelVisibility() => ActiveFilesPanelViewModel.SearchViewModel.ToggleSearch();

        private void SubscribeToEvents(IFilesPanelViewModel filesPanelViewModel)
        {
            filesPanelViewModel.Activated += FilesPanelViewModelOnActivatedEvent;
            filesPanelViewModel.CurrentDirectoryChanged += FilesPanelViewModelOnCurrentDirectoryChanged;
        }

        private void FilesPanelViewModelOnActivatedEvent(object sender, EventArgs e)
        {
            var filesPanelViewModel = (IFilesPanelViewModel) sender;
            if (filesPanelViewModel == ActiveFilesPanelViewModel)
            {
                return;
            }

            SwapViewModels();
            UpdateCurrentDirectory();
            DeactivateInactiveViewModel();
        }

        private void FilesPanelViewModelOnCurrentDirectoryChanged(object sender, EventArgs e)
        {
            var filesPanelViewModel = (IFilesPanelViewModel) sender;

            _directoryService.SelectedDirectory = filesPanelViewModel.CurrentDirectory;
        }

        private void SubscribeToEvents() =>
            _directoryService.SelectedDirectoryChanged += DirectoryServiceOnSelectedDirectoryChanged;

        private void UnsubscribeFromEvents() =>
            _directoryService.SelectedDirectoryChanged -= DirectoryServiceOnSelectedDirectoryChanged;


        private void DirectoryServiceOnSelectedDirectoryChanged(object sender, SelectedDirectoryChangedEventArgs e)
        {
            if (ActiveFilesPanelViewModel is null)
            {
                return;
            }

            ActiveFilesPanelViewModel.CurrentDirectory = e.NewDirectory;
        }

        private void SwapViewModels() =>
            (InactiveFilesPanelViewModel, ActiveFilesPanelViewModel) =
            (ActiveFilesPanelViewModel, InactiveFilesPanelViewModel);

        private void UpdateCurrentDirectory()
        {
            UnsubscribeFromEvents();
            _directoryService.SelectedDirectory = ActiveFilesPanelViewModel.CurrentDirectory;
            SubscribeToEvents();
        }

        private void DeactivateInactiveViewModel() => InactiveFilesPanelViewModel.Deactivate();
    }
}