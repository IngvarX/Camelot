using System;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Mediator.Interfaces;
using Camelot.Services.EventArgs;
using Camelot.Services.Interfaces;
using Camelot.ViewModels.MainWindow;
using DynamicData;

namespace Camelot.Mediator.Implementations
{
    public class FilesOperationsMediator : IFilesOperationsMediator
    {
        private readonly IDirectoryService _directoryService;
        private readonly IOperationsService _operationsService;

        private FilesPanelViewModel _activeViewModel;
        private FilesPanelViewModel _inactiveViewModel;

        private string OutputDirectory => _inactiveViewModel.CurrentDirectory;

        public FilesOperationsMediator(
            IDirectoryService directoryService,
            IOperationsService operationsService)
        {
            _directoryService = directoryService;
            _operationsService = operationsService;

            directoryService.SelectedDirectoryChanged += DirectoryServiceOnSelectedDirectoryChanged;
        }

        public void Register(FilesPanelViewModel activeFilesPanelViewModel, FilesPanelViewModel inactiveFilesPanelViewModel)
        {
            (_activeViewModel, _inactiveViewModel) = (activeFilesPanelViewModel, inactiveFilesPanelViewModel);

            SubscribeToEvents(_activeViewModel);
            SubscribeToEvents(_inactiveViewModel);

            UpdateCurrentDirectory();
        }

        public void EditSelectedFiles()
        {
            _operationsService.EditSelectedFiles();
        }

        public async Task CopySelectedFilesAsync()
        {
            await _operationsService.CopySelectedFilesAsync(OutputDirectory);
        }

        public async Task MoveSelectedFilesAsync()
        {
            await _operationsService.MoveSelectedFilesAsync(OutputDirectory);
        }

        public void CreateNewDirectory(string directoryName)
        {
            _operationsService.CreateDirectory(directoryName);
        }

        public Task RemoveSelectedFilesAsync()
        {
            return _operationsService.RemoveSelectedFilesAsync();
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