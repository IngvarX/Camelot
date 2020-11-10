using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Windows
{
    public class WindowsResourceOpeningService : IResourceOpeningService
    {
        private readonly IProcessService _processService;

        public WindowsResourceOpeningService(IProcessService processService)
        {
            _processService = processService;
        }

        public void Open(string resource) =>
            OpenWith("explorer", "{0}", resource);

        public void OpenWith(string command, string arguments, string resource)
        {
            if (resource.Any(char.IsWhiteSpace))
            {
                resource = "\"" + resource + "\"";
            }

            _processService.Run(command, string.Format(arguments, resource).TrimStart());
        }
    }
}