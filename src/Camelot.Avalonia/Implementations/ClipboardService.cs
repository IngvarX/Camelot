using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;
using Avalonia;
using Avalonia.Input.Platform;

namespace ApplicationDispatcher.Implementations
{
    public class ClipboardService : IClipboardService
    {
        private static IClipboard AvaloniaClipboard => Application.Current.Clipboard;

        public Task<string> GetTextAsync() => AvaloniaClipboard.GetTextAsync();

        public Task SetTextAsync(string text) => AvaloniaClipboard.SetTextAsync(text);
    }
}