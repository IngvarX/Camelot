using System.Collections.Generic;
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
        private readonly ArchiveType _archiveType;
        private readonly WriterOptions _options;

        public ArchiveWriter(
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
    }
}