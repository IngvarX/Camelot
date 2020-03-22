using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Camelot.Services.Operations.Implementations
{
    public class CopyOperation : OperationBase
    {
        private readonly string _sourceFile;
        private readonly string _destinationFile;

        public CopyOperation(string sourceFile, string destinationFile)
        {
            if (string.IsNullOrWhiteSpace(sourceFile))
            {
                throw new ArgumentNullException(nameof(sourceFile));
            }

            if (string.IsNullOrWhiteSpace(destinationFile))
            {
                throw new ArgumentNullException(nameof(destinationFile));
            }

            _sourceFile = sourceFile;
            _destinationFile = destinationFile;
        }

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (File.Exists(_sourceFile))
            {
                CopyFile(_sourceFile, _destinationFile);
            }
            else
            {
                // TODO: split in operation factory?
                CopyDirectoryRecursive(_sourceFile, _destinationFile);
            }

            FireOperationFinishedEvent();

            return Task.CompletedTask;
        }

        private static void CopyDirectoryRecursive(string sourceDirectory, string destinationDirectory)
        {
            var source = new DirectoryInfo(sourceDirectory);
            var dirs = source.GetDirectories();

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            var files = source.GetFiles();
            foreach (var file in files)
            {
                var tempPath = Path.Combine(destinationDirectory, file.Name);
                file.CopyTo(tempPath, false);
            }

            foreach (var directoryInfo in dirs)
            {
                var tempPath = Path.Combine(destinationDirectory, directoryInfo.Name);

                CopyDirectoryRecursive(directoryInfo.FullName, tempPath);
            }
        }

        private static void CopyFile(string source, string destination)
        {
            File.Copy(source, destination);
        }
    }
}