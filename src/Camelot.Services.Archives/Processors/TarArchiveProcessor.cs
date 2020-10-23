using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers.Tar;

namespace Camelot.Services.Archives.Processors
{
    public class TarArchiveProcessor : IArchiveProcessor
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;

        public TarArchiveProcessor(
            IFileService fileService,
            IDirectoryService directoryService)
        {
            _fileService = fileService;
            _directoryService = directoryService;
        }

        public async Task PackAsync(IReadOnlyList<string> files, IReadOnlyList<string> directories,
            string sourceDirectory, string outputFile)
        {
            await using var fileStream = _fileService.OpenWrite(outputFile);
            using var tarArchive = TarArchive.Create();

            var filesInDirectories = directories.SelectMany(d => _directoryService.GetFilesRecursively(d));
            foreach (var file in files.Concat(filesInDirectories))
            {
                tarArchive.AddEntry(file, file);
            }

            tarArchive.SaveTo(fileStream, new TarWriterOptions(CompressionType.None, true));
        }

        public Task ExtractAsync(string archivePath, string outputDirectory)
        {
            using var tarArchive = TarArchive.Open(archivePath);
            using var reader = tarArchive.ExtractAllEntries();

            var options = new ExtractionOptions
            {
                ExtractFullPath = true,
                Overwrite = true
            };

            reader.WriteAllToDirectory(outputDirectory, options);

            return Task.CompletedTask;
        }
    }
}