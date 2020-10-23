using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Archives.Interfaces;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

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
            using var gZipStream = new GZipOutputStream(fileStream);
            var tarArchive = TarArchive.CreateOutputTarArchive(gZipStream);

            // tarArchive.RootPath = sourceDirectory.Replace('\\', '/');
            // if (tarArchive.RootPath.EndsWith("/"))
            //     tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);
            //
            // AddDirectoryFilesToTar(tarArchive, sourceDirectory, true);

            return Task.CompletedTask;
        }

        public Task ExtractAsync(string archivePath, string outputDirectory)
        {
            using var inStream = _fileService.OpenRead(archivePath);
            using var gzipStream = _streamFactory.CreateInputStream(inStream);
            using var tarArchive = TarArchive.CreateInputTarArchive(gzipStream, Encoding.Default);

            tarArchive.ExtractContents(outputDirectory);

            return Task.CompletedTask;
        }
    }
}