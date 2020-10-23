using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using ICSharpCode.SharpZipLib.Tar;

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
            using var tarArchive = TarArchive.CreateOutputTarArchive(fileStream, Encoding.Default);

            var filesInDirectories = directories.SelectMany(d => _directoryService.GetFilesRecursively(d));
            foreach (var file in files.Concat(filesInDirectories))
            {
                var tarEntry = TarEntry.CreateEntryFromFile(file);

                tarArchive.WriteEntry(tarEntry, true);
            }
        }

        public async Task ExtractAsync(string archivePath, string outputDirectory)
        {
            await using var fileStream = _fileService.OpenRead(archivePath);
            using var tarArchive = TarArchive.CreateInputTarArchive(fileStream, Encoding.Default);

            tarArchive.ExtractContents(outputDirectory);
        }
    }
}