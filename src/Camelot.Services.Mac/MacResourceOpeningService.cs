using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Mac
{
    public class MacResourceOpeningService : IResourceOpeningService
    {
        private const string OpenCommand = "open";

        private readonly IProcessService _processService;

        public MacResourceOpeningService(
            IProcessService processService)
        {
            _processService = processService;
        }

        public void Open(string resource)
        {
            var arguments = resource.EndsWith(".app") ? $"-a \"{resource}\"" : $"\"{resource}\"";

            _processService.Run(OpenCommand, arguments);
        }

        public void OpenWith(string command, string arguments, string resource)
        {
            // Macos uses following file opening format:
            // open -a "<APP>" "<RESOURCE>"
            var escapedArguments = $"-a \"{command}\" \"{resource}\"";

            _processService.Run(OpenCommand, escapedArguments);
        }
    }
}