using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.Avalonia.Interfaces
{
    public interface IClipboardService
    {
        Task<string> GetTextAsync();

        Task<List<string>> GetFilesAsync();

        Task SetTextAsync(string text);

        Task SetFilesAsync(IReadOnlyList<string> files);
    }
}