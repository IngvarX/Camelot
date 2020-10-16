using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using ICSharpCode.SharpZipLib.Tar;

namespace Camelot.Services.Archives
{
    public class TarArchiveProcessor : IArchiveProcessor
    {
        private readonly IFileService _fileService;

        public TarArchiveProcessor(IFileService fileService)
        {
            _fileService = fileService;
        }

        public Task PackAsync(IReadOnlyList<string> nodes, string outputFile)
        {
            throw new System.NotImplementedException();
        }

        public async Task UnpackAsync(string archivePath, string outputDirectory)
        {
            await using var fileStream = _fileService.OpenRead(archivePath);

            using var tarArchive = TarArchive.CreateInputTarArchive(fileStream, Encoding.Default);
            tarArchive.ExtractContents(outputDirectory);
            tarArchive.Close();
        }
    }
}