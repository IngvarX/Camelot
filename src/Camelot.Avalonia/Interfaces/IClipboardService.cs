using System.Threading.Tasks;

namespace Camelot.Avalonia.Interfaces
{
    public interface IClipboardService
    {
        Task<string> GetTextAsync();

        Task SetTextAsync(string text);
    }
}