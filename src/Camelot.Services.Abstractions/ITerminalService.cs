using Camelot.Services.Abstractions.Models.State;

namespace Camelot.Services.Abstractions
{
    public interface ITerminalService
    {
        void Open(string directory);

        TerminalSettingsStateModel GetTerminalSettings();

        void SetTerminalSettings(TerminalSettingsStateModel terminalSettingsState);
    }
}