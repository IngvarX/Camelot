using Camelot.Services.Linux.Interfaces;

namespace Camelot.Services.Linux
{
    public class ShellCommandWrappingService : IShellCommandWrappingService
    {
        public (string, string) WrapWithNohup(string command, string arguments) =>
            ("bash", $"-c \"nohup {command} {arguments} >/dev/null 2>&1\"");
    }
}