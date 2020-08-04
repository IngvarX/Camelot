using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.Services.Abstractions
{
    public interface IClipboardOperationsService
    {
        Task CopyFilesAsync(IReadOnlyList<string> files);

        Task PasteFilesAsync(string destinationDirectory);
    }
}