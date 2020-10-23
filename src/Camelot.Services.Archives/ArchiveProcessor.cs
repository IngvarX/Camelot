using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace Camelot.Services.Archives
{
    public class ArchiveProcessor : IArchiveProcessor
    {
        private readonly IFileService _fileService;
        private readonly ArchiveType _archiveType;
        private readonly WriterOptions _options;

        public ArchiveProcessor(
            IFileService fileService,
            ArchiveType archiveType,
            WriterOptions options)
        {
            _fileService = fileService;
            _archiveType = archiveType;
            _options = options;
        }

        public async Task PackAsync(IReadOnlyList<string> files, IReadOnlyList<string> directories,
            string sourceDirectory, string outputFile)
        {
            await using var outStream = _fileService.OpenWrite(outputFile);
            using var writer = WriterFactory.Open(outStream, _archiveType, _options);

            foreach (var file in files)
            {
                writer.Write(file, file);
            }
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