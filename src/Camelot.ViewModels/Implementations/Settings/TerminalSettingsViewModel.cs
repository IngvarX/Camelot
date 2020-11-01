using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Interfaces.Settings;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Settings
{
    public class TerminalSettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly ITerminalService _terminalService;

        private string _initialCommandText;
        private string _initialCommandArguments;
        private bool _isActivated;

        [Reactive]
        public string TerminalCommandText { get; set; }

        [Reactive]
        public string TerminalCommandArguments { get; set; }

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
            var settings = new TerminalSettingsStateModel
            {
                Command = TerminalCommandText,
                Arguments = TerminalCommandArguments
            };

            _terminalService.SetTerminalSettings(settings);
        }
    }
}