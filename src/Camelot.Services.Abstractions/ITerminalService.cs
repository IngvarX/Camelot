using Camelot.DataAccess.Models;

namespace Camelot.Services.Abstractions
{
    public interface ITerminalService
    {
        void Open(string directory);

        TerminalSettings GetTerminalSettings();

        void SetTerminalSettings(TerminalSettings terminalSettings);
    }
}