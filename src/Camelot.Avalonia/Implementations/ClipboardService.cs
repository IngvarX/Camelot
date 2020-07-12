using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input.Platform;
using Camelot.Avalonia.Interfaces;

namespace Camelot.Avalonia.Implementations
{
    public class ClipboardService : IClipboardService
    {
        private static IClipboard AvaloniaClipboard => Application.Current.Clipboard;

        public Task<string> GetTextAsync() => AvaloniaClipboard.GetTextAsync();

        public Task SetTextAsync(string text) => AvaloniaClipboard.SetTextAsync(text);
    }
}