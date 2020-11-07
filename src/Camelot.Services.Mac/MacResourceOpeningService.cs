using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Mac
{
    public class MacResourceOpeningService : IResourceOpeningService
    {
        private readonly IProcessService _processService;

        public MacResourceOpeningService(
            IProcessService processService)
        {
            _processService = processService;
        }

        public void Open(string resource)
        {
            const string command = "open";
            var arguments = resource.EndsWith(".app") ? $"-a \"{resource}\"" : $"\"{resource}\"";

            _processService.Run(command, arguments);
        }

        public void OpenWith(string command, string resource)
        {
            throw new System.NotImplementedException();
        }
    }
}