using System.Threading.Tasks;

namespace ApplicationDispatcher.Interfaces
{
    public interface IClipboardService
    {
        Task<string> GetTextAsync();

        Task SetTextAsync(string text);
    }
}