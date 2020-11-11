using Camelot.Services.Abstractions.Behaviors;

namespace Camelot.Services.Mac
{
    public class MacDirectoryOpeningBehavior : IFileSystemNodeOpeningBehavior
    {
        private readonly IFileSystemNodeOpeningBehavior _fileOpeningBehavior;
        private readonly IFileSystemNodeOpeningBehavior _directoryOpeningBehavior;

        public MacDirectoryOpeningBehavior(
            IFileSystemNodeOpeningBehavior fileOpeningBehavior,
            IFileSystemNodeOpeningBehavior directoryOpeningBehavior)
        {
            _fileOpeningBehavior = fileOpeningBehavior;
            _directoryOpeningBehavior = directoryOpeningBehavior;
        }

        public void Open(string directory)
        {
            var behavior = GetBehavior(directory);

            behavior.Open(directory);
        }

        public void OpenWith(string command, string arguments, string directory)
        {
            var behavior = GetBehavior(directory);

            behavior.OpenWith(command, arguments, directory);
        }

        // .app directory is Macos application, so open it as a file
        private IFileSystemNodeOpeningBehavior GetBehavior(string directory) =>
            directory.EndsWith(".app") ? _fileOpeningBehavior : _directoryOpeningBehavior;
    }
}