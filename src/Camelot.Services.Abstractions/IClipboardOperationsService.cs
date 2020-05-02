using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.Services.Abstractions
{
    public interface IClipboardOperationsService
    {
        Task CopyFilesAsync(IReadOnlyCollection<string> files);

        Task PasteFilesAsync(string destinationDirectory);
    }
}