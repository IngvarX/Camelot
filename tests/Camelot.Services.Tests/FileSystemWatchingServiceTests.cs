using System.IO;
using Camelot.FileSystemWatcher.Interfaces;
using Camelot.Services.Abstractions;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class FileSystemWatchingServiceTests
    {
        private const string FileName = "File.txt";
        private const string NewFileName = "NewFile.txt";

        private readonly Mock<IFileSystemWatcher> _fileSystemWatcherMock;
        private readonly IFileSystemWatchingService _fileSystemWatchingService;

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

        private static string FilePath => Path.Combine(CurrentDirectory, FileName);

        private static string NewFilePath => Path.Combine(CurrentDirectory, NewFileName);

        public FileSystemWatchingServiceTests()
        {
            _fileSystemWatcherMock = new Mock<IFileSystemWatcher>();

            var factoryMock = new Mock<IFileSystemWatcherFactory>();
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
                Assert.True(eventArgs.NewName == NewFilePath);
            };

            _fileSystemWatchingService.StartWatching(CurrentDirectory);

            var args = new RenamedEventArgs(WatcherChangeTypes.Renamed, CurrentDirectory, NewFileName, FileName);
            _fileSystemWatcherMock.Raise(m => m.Renamed += null, args);

            Assert.True(callbackCalled);
        }

        [Fact]
        public void TestCallbackNotCalledWithoutSubscription()
        {
            var isCallbackCalled = false;
            _fileSystemWatchingService.NodeRenamed += (sender, eventArgs) => isCallbackCalled = true;

            var args = new RenamedEventArgs(WatcherChangeTypes.Renamed, CurrentDirectory, FileName, FileName);
            _fileSystemWatcherMock.Raise(m => m.Renamed += null, args);

            Assert.False(isCallbackCalled);
        }

        [Fact]
        public void TestCallbackNotCalledAfterUnsubscription()
        {
            var isCallbackCalled = false;
            _fileSystemWatchingService.NodeRenamed += (sender, eventArgs) => isCallbackCalled = true;

            _fileSystemWatchingService.StartWatching(CurrentDirectory);
            _fileSystemWatchingService.StopWatching(CurrentDirectory);

            var args = new RenamedEventArgs(WatcherChangeTypes.Renamed, CurrentDirectory, FileName, FileName);
            _fileSystemWatcherMock.Raise(m => m.Renamed += null, args);

            Assert.False(isCallbackCalled);
        }

        [Fact]
        public void TestMultipleSubscriptions()
        {
            var callbackCallsCount = 0;
            _fileSystemWatchingService.NodeRenamed += (sender, eventArgs) => callbackCallsCount++;

            _fileSystemWatchingService.StopWatching(CurrentDirectory);

            for (var i = 0; i < 10; i++)
            {
                _fileSystemWatchingService.StartWatching(CurrentDirectory);
            }

            var args = new RenamedEventArgs(WatcherChangeTypes.Renamed, CurrentDirectory, FileName, FileName);
            _fileSystemWatcherMock.Raise(m => m.Renamed += null, args);

            Assert.Equal(1, callbackCallsCount);

            _fileSystemWatchingService.StopWatching(CurrentDirectory);
            _fileSystemWatcherMock.Raise(m => m.Renamed += null, args);

            Assert.Equal(2, callbackCallsCount);
        }
    }
}