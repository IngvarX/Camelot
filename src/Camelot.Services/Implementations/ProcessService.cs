using System.Diagnostics;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class ProcessService : IProcessService
    {
        public void Run(string command) => Process.Start(command);

        public void Run(string command, string arguments)
        {
            var processStartInfo = new ProcessStartInfo(command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                Arguments = arguments
            };

            var process = new Process { StartInfo = processStartInfo };

            process.Start();
        }
    }
}