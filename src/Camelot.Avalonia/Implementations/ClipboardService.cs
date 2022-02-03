using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Camelot.Avalonia.Interfaces;

namespace Camelot.Avalonia.Implementations;

public class ClipboardService : IClipboardService
{
    private static IClipboard AvaloniaClipboard => Application.Current.Clipboard;

    public Task<string> GetTextAsync() => AvaloniaClipboard.GetTextAsync();

    public async Task<IReadOnlyList<string>> GetFilesAsync()
    {
        var data = await AvaloniaClipboard.GetDataAsync(DataFormats.FileNames);

        return (List<string>) data;
    }

    public Task SetTextAsync(string text) => AvaloniaClipboard.SetTextAsync(text);

    public async Task SetFilesAsync(IReadOnlyList<string> files)
    {
        var dataObject = new DataObject();
        dataObject.Set(DataFormats.FileNames, files);

        await AvaloniaClipboard.SetDataObjectAsync(dataObject);
    }
}