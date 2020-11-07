using System.Text.RegularExpressions;
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
            OpenWith("explorer", string.Empty, resource);

        public void OpenWith(string command, string arguments, string resource)
        {
            var hasPlaceholder = Regex.IsMatch(arguments, "\"{\\d+}\"", RegexOptions.Compiled);
            if (hasPlaceholder == false)
            {
                arguments = $"{arguments} \"{{0}}\"";
            }

            _processService.Run(command, string.Format(arguments, resource));
        }
    }
}