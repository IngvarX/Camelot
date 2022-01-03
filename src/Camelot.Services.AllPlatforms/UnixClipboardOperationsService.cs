using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.AllPlatforms;

public class UnixClipboardOperationsService : IClipboardOperationsService
{
    private const string UrlPrefix = "file://";

    private readonly IClipboardService _clipboardService;
    private readonly IOperationsService _operationsService;
    private readonly IEnvironmentService _environmentService;

    public UnixClipboardOperationsService(
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
        var selectedFilesString = await _clipboardService.GetTextAsync();
        if (string.IsNullOrWhiteSpace(selectedFilesString))
        {
            return Array.Empty<string>();
        }

        var startIndex = UrlPrefix.Length;

        return selectedFilesString
            .Split(_environmentService.NewLine)
            .Where(t => t.StartsWith(UrlPrefix))
            .Select(f => f[startIndex..])
            .ToArray();
    }
}