using System.Diagnostics;
using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Environment.Implementations
{
    public class ProcessService : IProcessService
    {
        public void Run(string command, string arguments)
        {
            var process = GetProcess(command, arguments);

            process.Start();
        }

        public async Task<string> ExecuteAndGetOutputAsync(string command, string arguments)
        {
            var process = GetProcess(command, arguments, true);
            process.Start();

            return await process.StandardOutput.ReadToEndAsync();
        }

        private static Process GetProcess(string command, string arguments, bool redirectOutput = false)
        {
            var processStartInfo = new ProcessStartInfo(command)
            {
                RedirectStandardOutput = redirectOutput,
                UseShellExecute = false,
                CreateNoWindow = false,
                Arguments = arguments
            };

            return new Process {StartInfo = processStartInfo};
        }
    }
}