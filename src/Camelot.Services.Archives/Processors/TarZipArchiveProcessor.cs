using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Archives.Interfaces;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Camelot.Services.Archives.Processors
{
    public class TarZipArchiveProcessor : IArchiveProcessor
    {
        private readonly IFileService _fileService;
        private readonly IStreamFactory _streamFactory;

        public TarZipArchiveProcessor(
            IFileService fileService,
            IStreamFactory streamFactory)
        {
            _fileService = fileService;
            _streamFactory = streamFactory;
        }

        public Task PackAsync(IReadOnlyList<string> files, IReadOnlyList<string> directories, string sourceDirectory,
            string outputFile)
        {
            using var fileStream = _fileService.OpenWrite(outputFile);
            // using var gZipStream = new GZipOutputStream(fileStream);
            // var tarArchive = TarArchive.CreateOutputTarArchive(gZipStream);

            // tarArchive.RootPath = sourceDirectory.Replace('\\', '/');
            // if (tarArchive.RootPath.EndsWith("/"))
            //     tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);
            //
            // AddDirectoryFilesToTar(tarArchive, sourceDirectory, true);

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