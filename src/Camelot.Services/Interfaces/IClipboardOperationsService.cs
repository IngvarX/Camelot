using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.Services.Interfaces
{
    public interface IClipboardOperationsService
    {
        Task CopyFilesAsync(IReadOnlyCollection<string> files);

        Task PasteFilesAsync(string destinationDirectory);
    }
}