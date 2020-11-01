namespace Camelot.Services.Abstractions.Models.State
{
    public class TerminalSettingsStateModel
    {
        public string Command { get; set; }

        public string Arguments { get; set; }

        public void Deconstruct(out string command, out string arguments)
        {
            command = Command;
            arguments = Arguments;
        }
    }
}