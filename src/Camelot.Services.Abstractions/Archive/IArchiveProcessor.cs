using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.Services.Abstractions.Archive
{
    public interface IArchiveProcessor
    {
        Task PackAsync(IReadOnlyList<string> nodes, string outputFile);

        Task UnpackAsync(string archivePath, string outputDirectory);
    }
}