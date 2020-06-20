using Camelot.DataAccess.Models;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Interfaces.Settings;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Settings
{
    public class TerminalSettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly ITerminalService _terminalService;

        private string _terminalCommandText;
        private string _terminalCommandArguments;
        private string _initialCommandText;
        private string _initialCommandArguments;
        private bool _isActivated;

        public string TerminalCommandText
        {
            get => _terminalCommandText;
            set => this.RaiseAndSetIfChanged(ref _terminalCommandText, value);
        }

        public string TerminalCommandArguments
        {
            get => _terminalCommandArguments;
            set => this.RaiseAndSetIfChanged(ref _terminalCommandArguments, value);
        }

        public bool IsChanged => _initialCommandText != TerminalCommandText ||
                                 _initialCommandArguments != TerminalCommandArguments;

        public TerminalSettingsViewModel(
            ITerminalService terminalService)
        {
            _terminalService = terminalService;
        }

        public void Activate()
        {
            if (_isActivated)
            {
                return;
            }

            _isActivated = true;

            var (command, arguments) = _terminalService.GetTerminalSettings();

            TerminalCommandText = _initialCommandText = command;
            TerminalCommandArguments = _initialCommandArguments = arguments;
        }

        public void SaveChanges()
        {
            var settings = new TerminalSettings
            {
                Command = TerminalCommandText,
                Arguments = TerminalCommandArguments
            };

            _terminalService.SetTerminalSettings(settings);
        }
    }
}