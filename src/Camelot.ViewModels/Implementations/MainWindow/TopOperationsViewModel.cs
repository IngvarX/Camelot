using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Interfaces.MainWindow;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public class TopOperationsViewModel : ViewModelBase, ITopOperationsViewModel
    {
        private readonly ITerminalService _terminalService;
        private readonly IDirectoryService _directoryService;

        public ICommand OpenTerminalCommand { get; }

        public TopOperationsViewModel(
            ITerminalService terminalService,
            IDirectoryService directoryService)
        {
            _terminalService = terminalService;
            _directoryService = directoryService;

            OpenTerminalCommand = ReactiveCommand.Create(OpenTerminal);
        }

        private void OpenTerminal() => _terminalService.Open(_directoryService.SelectedDirectory);
    }
}