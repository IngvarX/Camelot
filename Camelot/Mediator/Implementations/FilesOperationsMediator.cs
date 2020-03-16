using System;
using System.Threading.Tasks;
using Camelot.Mediator.Interfaces;
using Camelot.Services.EventArgs;
using Camelot.Services.Interfaces;
using Camelot.ViewModels.MainWindow;

namespace Camelot.Mediator.Implementations
{
    public class FilesOperationsMediator : IFilesOperationsMediator
    {
        private readonly IOperationsService _operationsService;

        private FilesPanelViewModel _activeViewModel;
        private FilesPanelViewModel _inactiveViewModel;

        private string OutputDirectory => _inactiveViewModel.CurrentDirectory;

        public FilesOperationsMediator(
            IDirectoryService directoryService,
            IOperationsService operationsService)
        {
            _operationsService = operationsService;

            directoryService.SelectedDirectoryChanged += DirectoryServiceOnSelectedDirectoryChanged;
        }

        public void Register(FilesPanelViewModel activeFilesPanelViewModel, FilesPanelViewModel inactiveFilesPanelViewModel)
        {
            (_activeViewModel, _inactiveViewModel) = (activeFilesPanelViewModel, inactiveFilesPanelViewModel);

            SubscribeToEvents(_activeViewModel);
            SubscribeToEvents(_inactiveViewModel);
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

            (_inactiveViewModel, _activeViewModel) = (_activeViewModel, _inactiveViewModel);
        }

        private void DirectoryServiceOnSelectedDirectoryChanged(object sender, SelectedDirectoryChangedEventArgs e)
        {
            _activeViewModel.CurrentDirectory = e.NewDirectory;
        }
    }
}