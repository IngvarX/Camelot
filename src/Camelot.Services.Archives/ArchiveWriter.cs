using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace Camelot.Services.Archives
{
    public class ArchiveWriter : IArchiveWriter
    {
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly IDirectoryService _directoryService;
        private readonly ArchiveType _archiveType;
        private readonly WriterOptions _options;

        public ArchiveWriter(
            IFileService fileService,
            IPathService pathService,
            IDirectoryService directoryService,
            ArchiveType archiveType,
            WriterOptions options)
        {
            _fileService = fileService;
            _pathService = pathService;
            _directoryService = directoryService;
            _archiveType = archiveType;
            _options = options;
        }

        public async Task PackAsync(IReadOnlyList<string> files, IReadOnlyList<string> directories,
            string sourceDirectory, string outputFile)
        {
            await using var outStream = _fileService.OpenWrite(outputFile);
            using var writer = WriterFactory.Open(outStream, _archiveType, _options);

            var allFiles = files.Concat(directories.SelectMany(d => _directoryService.GetFilesRecursively(d)));

            foreach (var file in allFiles)
            {
                var entryPath = _pathService.GetRelativePath(sourceDirectory, file);
                writer.Write(entryPath, file);
            }
        }
    }
}