using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Archives.Interfaces;

namespace Camelot.Services.Archives.Processors
{
    public class SingleFileZipArchiveProcessor : IArchiveProcessor
    {
        private readonly IFileService _fileService;
        private readonly IFileNameGenerationService _fileNameGenerationService;
        private readonly IPathService _pathService;
        private readonly IStreamFactory _streamFactory;

        public SingleFileZipArchiveProcessor(
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

        public async Task PackAsync(IReadOnlyList<string> files, IReadOnlyList<string> directories,
            string sourceDirectory, string outputFile)
        {
            if (directories.Any() || files.Count > 1)
            {
                throw new InvalidOperationException("Gzip can be used for single file only!");
            }

            await using var inStream = _fileService.OpenRead(files.Single());
            await using var outStream = _fileService.OpenWrite(outputFile);
            await using var zipStream = _streamFactory.CreateOutputStream(outStream);

            await zipStream.CopyToAsync(outStream);
        }

        public async Task ExtractAsync(string archivePath, string outputDirectory)
        {
            await using var inStream = _fileService.OpenRead(archivePath);
            await using var zipStream = _streamFactory.CreateInputStream(inStream);

            var outputFilePath = GetOutputFilePath(archivePath, outputDirectory);
            await using var outStream = _fileService.OpenWrite(outputFilePath);

            await zipStream.CopyToAsync(outStream);
        }

        private string GetOutputFilePath(string archivePath, string outputDirectory)
        {
            var archiveFileName = _pathService.GetFileNameWithoutExtension(archivePath);
            var outputFilePath = _pathService.Combine(outputDirectory, archiveFileName);

            return _fileNameGenerationService.GenerateFullName(outputFilePath);
        }
    }
}