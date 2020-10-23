using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Archives.Interfaces;
using ICSharpCode.SharpZipLib.Core;

namespace Camelot.Services.Archives.Processors
{
    public class SingleFileZipArchiveProcessor : IArchiveProcessor
    {
        private const int BufferSize = 4096;

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

        public Task PackAsync(IReadOnlyList<string> files, IReadOnlyList<string> directories,
            string sourceDirectory, string outputFile)
        {
            if (directories.Any() || files.Count > 1)
            {
                throw new InvalidOperationException("Gzip can be used for single file only!");
            }

            using var inStream = _fileService.OpenRead(files.Single());
            using var outStream = _fileService.OpenWrite(outputFile);
            using var zipStream = _streamFactory.CreateOutputStream(outStream);

            var dataBuffer = new byte[BufferSize];
            StreamUtils.Copy(inStream, zipStream, dataBuffer);

            return Task.CompletedTask;
        }

        public Task ExtractAsync(string archivePath, string outputDirectory)
        {
            using var inStream = _fileService.OpenRead(archivePath);
            using var zipStream = _streamFactory.CreateInputStream(inStream);

            var outputFilePath = GetOutputFilePath(archivePath, outputDirectory);
            using var fileStream = _fileService.OpenWrite(outputFilePath);

            var dataBuffer = new byte[BufferSize];
            StreamUtils.Copy(zipStream, fileStream, dataBuffer);

            return Task.CompletedTask;
        }

        private string GetOutputFilePath(string archivePath, string outputDirectory)
        {
            var archiveFileName = _pathService.GetFileNameWithoutExtension(archivePath);
            var outputFilePath = _pathService.Combine(outputDirectory, archiveFileName);

            return _fileNameGenerationService.GenerateFullName(outputFilePath);
        }
    }
}