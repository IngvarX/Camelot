using System.IO;
using Camelot.FileSystemWatcherWrapper.Interfaces;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Tests
{
    public class FileSystemWatchingServiceTests
    {
        private const string FileName = "File.txt";

        private readonly Mock<IFileSystemWatcherWrapper> _fileSystemWatcherMock;
        private readonly IFileSystemWatchingService _fileSystemWatchingService;

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

        private static string FilePath => Path.Combine(CurrentDirectory, FileName);

        public FileSystemWatchingServiceTests()
        {
            _fileSystemWatcherMock = new Mock<IFileSystemWatcherWrapper>();

            var factoryMock = new Mock<IFileSystemWatcherWrapperFactory>();
            factoryMock
                .Setup(m => m.Create(CurrentDirectory))
                .Returns(_fileSystemWatcherMock.Object);

            _fileSystemWatchingService = new FileSystemWatchingService(factoryMock.Object);
        }

        [Fact]
        public void TestFileRemoved()
        {
            var callbackCalled = false;
            _fileSystemWatchingService.NodeDeleted += (sender, eventArgs) =>
            {
                callbackCalled = true;
                Assert.True(eventArgs.Node == FilePath);
            };

            _fileSystemWatchingService.StartWatching(CurrentDirectory);

            var args = new FileSystemEventArgs(WatcherChangeTypes.Deleted, CurrentDirectory, FileName);
            _fileSystemWatcherMock.Raise(m => m.Deleted += null, args);

            Assert.True(callbackCalled);
        }

        [Fact]
        public void TestFileCreated()
        {
            var callbackCalled = false;
            _fileSystemWatchingService.NodeCreated += (sender, eventArgs) =>
            {
                callbackCalled = true;
                Assert.True(eventArgs.Node == FilePath);
            };

            _fileSystemWatchingService.StartWatching(CurrentDirectory);

            var args = new FileSystemEventArgs(WatcherChangeTypes.Created, CurrentDirectory, FileName);
            _fileSystemWatcherMock.Raise(m => m.Created += null, args);

            Assert.True(callbackCalled);
        }

        [Fact]
        public void TestFileChanged()
        {
            var callbackCalled = false;
            _fileSystemWatchingService.NodeChanged += (sender, eventArgs) =>
            {
                callbackCalled = true;
                Assert.True(eventArgs.Node == FilePath);
            };

            _fileSystemWatchingService.StartWatching(CurrentDirectory);

            var args = new FileSystemEventArgs(WatcherChangeTypes.Changed, CurrentDirectory, FileName);
            _fileSystemWatcherMock.Raise(m => m.Changed += null, args);

            Assert.True(callbackCalled);
        }

        [Fact]
        public void TestFileRenamed()
        {
            var callbackCalled = false;
            _fileSystemWatchingService.NodeRenamed += (sender, eventArgs) =>
            {
                callbackCalled = true;
                Assert.True(eventArgs.Node == FilePath);
            };

            _fileSystemWatchingService.StartWatching(CurrentDirectory);

            var args = new RenamedEventArgs(WatcherChangeTypes.Renamed, CurrentDirectory, FileName, FileName);
            _fileSystemWatcherMock.Raise(m => m.Renamed += null, args);

            Assert.True(callbackCalled);
        }
    }
}