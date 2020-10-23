using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Archives.Interfaces;

namespace Camelot.Services.Archives
{
    public class SingleFileZipArchiveReader : IArchiveReader
    {
        private readonly IFileService _fileService;
        private readonly IFileNameGenerationService _fileNameGenerationService;
        private readonly IPathService _pathService;
        private readonly IStreamFactory _streamFactory;

        public SingleFileZipArchiveReader(
            IFileService fileService,
            IFileNameGenerationService fileNameGenerationService,
            IPathService pathService,
            IStreamFactory streamFactory)
        {
            _fileService = fileService;
            _fileNameGenerationService = fileNameGenerationService;
            _pathService = pathService;
            _streamFactory = streamFactory;
        }

        public async Task ExtractAsync(string archivePath, string outputDirectory)
        {
            await using var inputStream = _fileService.OpenRead(archivePath);
            await using var zipStream = _streamFactory.Create(inputStream);

            var outputFilePath = GetOutputFilePath(archivePath, outputDirectory);
            await using var outputStream = _fileService.OpenWrite(outputFilePath);

            await zipStream.CopyToAsync(outputStream);
        }

        private string GetOutputFilePath(string archivePath, string outputDirectory)
        {
            var archiveFileName = _pathService.GetFileNameWithoutExtension(archivePath);
            var outputFilePath = _pathService.Combine(outputDirectory, archiveFileName);

            return _fileNameGenerationService.GenerateFullName(outputFilePath);
        }
    }
}