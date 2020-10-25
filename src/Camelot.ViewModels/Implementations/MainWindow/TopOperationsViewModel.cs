using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public class TopOperationsViewModel : ViewModelBase, ITopOperationsViewModel
    {
        private readonly ITerminalService _terminalService;
        private readonly IDirectoryService _directoryService;
        private readonly IFilesOperationsMediator _filesOperationsMediator;
        private readonly IDialogService _dialogService;
        private readonly IPathService _pathService;
        private readonly IArchiveService _archiveService;
        private readonly INodesSelectionService _nodesSelectionService;
        private readonly ISystemDialogService _systemDialogService;

        public ICommand PackCommand { get; }

        public ICommand ExtractCommand { get; }

        public ICommand SearchCommand { get; }

        public ICommand OpenTerminalCommand { get; }

        public TopOperationsViewModel(
            ITerminalService terminalService,
            IDirectoryService directoryService,
            IFilesOperationsMediator filesOperationsMediator,
            IDialogService dialogService,
            IPathService pathService,
            IArchiveService archiveService,
            INodesSelectionService nodesSelectionService,
            ISystemDialogService systemDialogService)
        {
            _terminalService = terminalService;
            _directoryService = directoryService;
            _filesOperationsMediator = filesOperationsMediator;
            _dialogService = dialogService;
            _pathService = pathService;
            _archiveService = archiveService;
            _nodesSelectionService = nodesSelectionService;
            _systemDialogService = systemDialogService;

            PackCommand = ReactiveCommand.CreateFromTask(PackAsync);
            ExtractCommand = ReactiveCommand.Create(ExtractAsync);
            SearchCommand = ReactiveCommand.Create(Search);
            OpenTerminalCommand = ReactiveCommand.Create(OpenTerminal);
        }

        private async Task PackAsync()
        {
            var selectedNodes = _nodesSelectionService
                .SelectedNodes
                .ToArray();
            if (!selectedNodes.Any())
            {
                return;
            }

            var defaultPath = _pathService.Combine(_directoryService.SelectedDirectory, selectedNodes.First());
            var parameter = new CreateArchiveNavigationParameter(defaultPath, selectedNodes.Length == 1);
            var dialogResult = await _dialogService.ShowDialogAsync<CreateArchiveDialogResult, CreateArchiveNavigationParameter>(
                nameof(CreateArchiveDialogViewModel), parameter);
            if (dialogResult is null)
            {
                return;
            }

            await _archiveService.PackAsync(selectedNodes, dialogResult.ArchivePath, dialogResult.ArchiveType);
        }

        private async Task ExtractAsync()
        {
            var selectedArchives = _nodesSelectionService
                .SelectedNodes
                .Where(_archiveService.CheckIfNodeIsArchive)
                .ToArray();
            if (!selectedArchives.Any())
            {
                return;
            }

            var directory = await _systemDialogService.GetDirectoryAsync();
            if (string.IsNullOrWhiteSpace(directory))
            {
                return;
            }

            foreach (var archive in selectedArchives)
            {
                await _archiveService.ExtractAsync(archive, directory);
            }
        }

        private void Search() => _filesOperationsMediator.ToggleSearchPanelVisibility();

        private void OpenTerminal() => _terminalService.Open(_directoryService.SelectedDirectory);
    }
}