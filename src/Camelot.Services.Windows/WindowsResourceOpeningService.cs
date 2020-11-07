using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Windows
{
    public class WindowsResourceOpeningService : IResourceOpeningService
    {
        private readonly IProcessService _processService;

        public WindowsResourceOpeningService(
            IProcessService processService)
        {
            _processService = processService;
        }

        public void Open(string resource) =>
            OpenWith("explorer", resource);

        public void OpenWith(string command, string resource) 
            => _processService.Run(command, $"\"{resource}\"");
    }
}