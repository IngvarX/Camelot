using System;
using Camelot.Mediator.Interfaces;
using Camelot.Services.EventArgs;
using Camelot.Services.Interfaces;
using Camelot.ViewModels.MainWindow;

namespace Camelot.Mediator.Implementations
{
    public class FilesOperationsMediator : IFilesOperationsMediator
    {
        private readonly IDirectoryService _directoryService;

        private FilesPanelViewModel _activeViewModel;
        private FilesPanelViewModel _inactiveViewModel;

        public string OutputDirectory => _inactiveViewModel.CurrentDirectory;

        public FilesOperationsMediator(
            IDirectoryService directoryService)
        {
            _directoryService = directoryService;

            directoryService.SelectedDirectoryChanged += DirectoryServiceOnSelectedDirectoryChanged;
        }

        public void Register(FilesPanelViewModel activeFilesPanelViewModel, FilesPanelViewModel inactiveFilesPanelViewModel)
        {
            (_activeViewModel, _inactiveViewModel) = (activeFilesPanelViewModel, inactiveFilesPanelViewModel);

            SubscribeToEvents(_activeViewModel);
            SubscribeToEvents(_inactiveViewModel);

            UpdateCurrentDirectory();
        }

        private void SubscribeToEvents(FilesPanelViewModel filesPanelViewModel)
        {
            filesPanelViewModel.ActivatedEvent += FilesPanelViewModelOnActivatedEvent;
        }

        private void FilesPanelViewModelOnActivatedEvent(object sender, EventArgs e)
        {
            var filesPanelViewModel = (FilesPanelViewModel) sender;
            if (filesPanelViewModel == _activeViewModel)
            {
                return;
            }

            SwapViewModels();
            UpdateCurrentDirectory();
            DeactivateInactiveViewModel();
        }

        private void DirectoryServiceOnSelectedDirectoryChanged(object sender, SelectedDirectoryChangedEventArgs e)
        {
            _activeViewModel.CurrentDirectory = e.NewDirectory;
        }

        private void SwapViewModels()
        {
            (_inactiveViewModel, _activeViewModel) = (_activeViewModel, _inactiveViewModel);
        }

        private void UpdateCurrentDirectory()
        {
            _directoryService.SelectedDirectory = _activeViewModel.CurrentDirectory;
        }

        private void DeactivateInactiveViewModel()
        {
            _inactiveViewModel.Deactivate();
        }
    }
}