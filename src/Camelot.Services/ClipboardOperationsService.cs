using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services
{
    public class ClipboardOperationsService : IClipboardOperationsService
    {
        private const string UrlPrefix = "file://";

        private readonly IClipboardService _clipboardService;
        private readonly IOperationsService _operationsService;
        private readonly IEnvironmentService _environmentService;

        public ClipboardOperationsService(
            IClipboardService clipboardService,
            IOperationsService operationsService,
            IEnvironmentService environmentService)
        {
            _clipboardService = clipboardService;
            _operationsService = operationsService;
            _environmentService = environmentService;
        }

        public async Task CopyFilesAsync(IReadOnlyList<string> files)
        {
            var selectedFilesString = string.Join(_environmentService.NewLine,
                files.Select(f => UrlPrefix + f));

            await _clipboardService.SetTextAsync(selectedFilesString);
        }

        public async Task PasteFilesAsync(string destinationDirectory)
        {
            var selectedFilesString = await _clipboardService.GetTextAsync();
            if (string.IsNullOrWhiteSpace(selectedFilesString))
            {
                return;
            }

            var startIndex = UrlPrefix.Length;
            var files = selectedFilesString
                .Split()
                .Where(t => t.StartsWith(UrlPrefix))
                .Select(f => f.Substring(startIndex))
                .ToArray();
            if (files.Any())
            {
                await _operationsService.CopyAsync(files, destinationDirectory);
            }
        }
    }
}