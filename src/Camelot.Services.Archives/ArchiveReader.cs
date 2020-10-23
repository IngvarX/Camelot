using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Camelot.Services.Archives
{
    public class ArchiveReader : IArchiveReader
    {
        private readonly IFileService _fileService;

        public ArchiveReader(
            IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task ExtractAsync(string archivePath, string outputDirectory)
        {
            await using var inStream = _fileService.OpenRead(archivePath);
            using var reader = ReaderFactory.Open(inStream);

            var options = new ExtractionOptions
            {
                ExtractFullPath = true,
                Overwrite = true
            };

            reader.WriteAllToDirectory(outputDirectory, options);
        }
    }
}