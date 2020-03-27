using System;
using Camelot.Services.EventArgs;
using Camelot.Services.Interfaces;
using Camelot.ViewModels.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Services.Implementations
{
    public class FilesOperationsMediator : IFilesOperationsMediator
    {
        private readonly IDirectoryService _directoryService;

        private IFilesPanelViewModel _activeViewModel;
        private IFilesPanelViewModel _inactiveViewModel;

        public string OutputDirectory => _inactiveViewModel.CurrentDirectory;

        public FilesOperationsMediator(
            IDirectoryService directoryService)
        {
            _directoryService = directoryService;

            directoryService.SelectedDirectoryChanged += DirectoryServiceOnSelectedDirectoryChanged;
        }

        public void Register(IFilesPanelViewModel activeFilesPanelViewModel, IFilesPanelViewModel inactiveFilesPanelViewModel)
        {
            (_activeViewModel, _inactiveViewModel) = (activeFilesPanelViewModel, inactiveFilesPanelViewModel);

            SubscribeToEvents(_activeViewModel);
            SubscribeToEvents(_inactiveViewModel);

            UpdateCurrentDirectory();
        }

        private void SubscribeToEvents(IFilesPanelViewModel filesPanelViewModel)
        {
            filesPanelViewModel.ActivatedEvent += FilesPanelViewModelOnActivatedEvent;
        }

        private void FilesPanelViewModelOnActivatedEvent(object sender, EventArgs e)
        {
            var filesPanelViewModel = (IFilesPanelViewModel) sender;
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