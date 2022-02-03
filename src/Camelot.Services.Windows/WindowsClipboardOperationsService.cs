using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Windows;

public class WindowsClipboardOperationsService : IClipboardOperationsService
{
    private readonly IClipboardService _clipboardService;
    private readonly IOperationsService _operationsService;

    public WindowsClipboardOperationsService(
        IClipboardService clipboardService,
        IOperationsService operationsService)
    {
        _clipboardService = clipboardService;
        _operationsService = operationsService;
    }

    public Task CopyFilesAsync(IReadOnlyList<string> files) => _clipboardService.SetFilesAsync(files);

    public async Task PasteFilesAsync(string destinationDirectory)
    {
        var files = await GetFilesAsync();
        if (files.Any())
        {
            await _operationsService.CopyAsync(files, destinationDirectory);
        }
    }

    public async Task<bool> CanPasteAsync()
    {
        var files = await GetFilesAsync();

        return files.Any();
    }

    private async Task<IReadOnlyList<string>> GetFilesAsync()
    {
        var files = await _clipboardService.GetFilesAsync();

        return files ?? Array.Empty<string>();
    }
}