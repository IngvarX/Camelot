using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class ClipboardOperationsService : IClipboardOperationsService
    {
        private const string UrlPrefix = "file://";

        private readonly IClipboardService _clipboardService;
        private readonly IOperationsService _operationsService;

        public ClipboardOperationsService(
            IClipboardService clipboardService,
            IOperationsService operationsService)
        {
            _clipboardService = clipboardService;
            _operationsService = operationsService;
        }

        public async Task CopyFilesAsync(IReadOnlyCollection<string> files)
        {
            var selectedFilesString = string.Join(Environment.NewLine, files.Select(f => UrlPrefix + f));

            await _clipboardService.SetTextAsync(selectedFilesString);
        }

        public async Task PasteSelectedFilesAsync(string destinationDirectory)
        {
            var selectedFilesString = await _clipboardService.GetTextAsync();
            var startIndex = UrlPrefix.Length;
            var files = selectedFilesString
                .Split()
                .Select(f => f.Substring(startIndex))
                .ToArray();

            await _operationsService.CopyFilesAsync(files, destinationDirectory);
        }
    }
}