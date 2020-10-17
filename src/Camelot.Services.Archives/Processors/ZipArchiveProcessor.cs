using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Camelot.Services.Archives.Processors
{
    public class ZipArchiveProcessor : IArchiveProcessor
    {
        private readonly IPathService _pathService;

        public ZipArchiveProcessor(
            IPathService pathService)
        {
            _pathService = pathService;
        }

        public Task PackAsync(IReadOnlyList<string> nodes, string outputFile)
        {
            var sourceDirectory = _pathService.GetCommonRootDirectory(nodes);
            var fastZip = Create();
            var scanFilter = Create(nodes);

            fastZip.CreateZip(outputFile, sourceDirectory, true, scanFilter, scanFilter);

            return Task.CompletedTask;
        }

        public Task ExtractAsync(string archivePath, string outputDirectory)
        {
            var fastZip = Create();

            fastZip.ExtractZip(archivePath, outputDirectory, FastZip.Overwrite.Always, null, null, null, true);

            return Task.CompletedTask;
        }

        private static FastZip Create() => new FastZip
        {
            CreateEmptyDirectories = true
        };

        private static IScanFilter Create(IReadOnlyList<string> nodes) =>
            new ScanFilter(nodes.ToHashSet());
    }
}