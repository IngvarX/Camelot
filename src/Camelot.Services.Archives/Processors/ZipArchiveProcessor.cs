using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Archive;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Camelot.Services.Archives.Processors
{
    public class ZipArchiveProcessor : IArchiveProcessor
    {
        public Task PackAsync(IReadOnlyList<string> files, IReadOnlyList<string> directories,
            string sourceDirectory, string outputFile)
        {
            var fastZip = Create();
            var scanFilter = Create(files.Concat(directories).ToHashSet());

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

        private static IScanFilter Create(ISet<string> nodes) =>
            new ScanFilter(nodes);
    }
}