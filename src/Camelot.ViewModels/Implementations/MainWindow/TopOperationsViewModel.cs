using System.Windows.Input;
using Camelot.Services.Abstractions;
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

        public ICommand SearchCommand { get; }

        public ICommand OpenTerminalCommand { get; }

        public TopOperationsViewModel(
            ITerminalService terminalService,
            IDirectoryService directoryService,
            IFilesOperationsMediator filesOperationsMediator)
        {
            _terminalService = terminalService;
            _directoryService = directoryService;
            _filesOperationsMediator = filesOperationsMediator;

            SearchCommand = ReactiveCommand.Create(Search);
            OpenTerminalCommand = ReactiveCommand.Create(OpenTerminal);
        }

        private void Search() => _filesOperationsMediator.ToggleSearchPanelVisibility();

        private void OpenTerminal() => _terminalService.Open(_directoryService.SelectedDirectory);
    }
}