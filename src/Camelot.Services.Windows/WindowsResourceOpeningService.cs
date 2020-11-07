using System.Text.RegularExpressions;
using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Windows
{
    public class WindowsResourceOpeningService : IResourceOpeningService
    {
        private readonly IProcessService _processService;
        private readonly IRegexService _regexService;

        public WindowsResourceOpeningService(
            IProcessService processService,
            IRegexService regexService)
        {
            _processService = processService;
            _regexService = regexService;
        }

        public void Open(string resource) =>
            OpenWith("explorer", string.Empty, resource);

        public void OpenWith(string command, string arguments, string resource)
        {
            var hasPlaceholder = _regexService.CheckIfMatches(arguments, "\"{\\d+}\"", RegexOptions.Compiled);
            if (!hasPlaceholder)
            {
                arguments = $"{arguments} \"{{0}}\"";
            }

            _processService.Run(command, string.Format(arguments, resource).TrimStart());
        }
    }
}