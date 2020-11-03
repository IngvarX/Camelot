using System.Threading.Tasks;

namespace Camelot.Services.Abstractions.Archive
{
    public interface IArchiveReader
    {
        Task ExtractAsync(string archivePath, string outputDirectory);
    }
}