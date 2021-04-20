using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.AllPlatforms
{
    public class FilesClipboardOperationsService : IClipboardOperationsService
    {
        private readonly IClipboardService _clipboardService;
        private readonly IOperationsService _operationsService;

        public FilesClipboardOperationsService(
            IClipboardService clipboardService,
            IOperationsService operationsService)
        {
            _clipboardService = clipboardService;
            _operationsService = operationsService;
        }

        public Task CopyFilesAsync(IReadOnlyList<string> files) => _clipboardService.SetFilesAsync(files);

        public async Task PasteFilesAsync(string destinationDirectory)
        {
            var files = await _clipboardService.GetFilesAsync();
            if (files?.Any() == true)
            {
                await _operationsService.CopyAsync(files, destinationDirectory);
            }
        }
    }
}