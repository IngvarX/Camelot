using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Camelot.Services.Archives.Processors
{
    public class ZipArchiveProcessor : IArchiveProcessor
    {
        private readonly IFileService _fileService;

        public ZipArchiveProcessor(IFileService fileService)
        {
            _fileService = fileService;
        }

        public Task PackAsync(IReadOnlyList<string> files, IReadOnlyList<string> directories,
            string sourceDirectory, string outputFile)
        {
            // var fastZip = Create();
            // var scanFilter = Create(files.Concat(directories).ToHashSet());
            //
            // fastZip.CreateZip(outputFile, sourceDirectory, true, scanFilter, scanFilter);

            return Task.CompletedTask;
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