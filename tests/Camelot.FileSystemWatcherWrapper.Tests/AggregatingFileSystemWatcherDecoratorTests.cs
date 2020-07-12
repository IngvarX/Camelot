using System.IO;
using System.Threading.Tasks;
using Camelot.FileSystemWatcher.Configuration;
using Camelot.FileSystemWatcher.Implementations;
using Camelot.FileSystemWatcher.Interfaces;
using Camelot.Services.Abstractions;
using Moq;
using Xunit;

namespace Camelot.FileSystemWatcherWrapper.Tests
{
    public class AggregatingFileSystemWatcherDecoratorTests
    {
        private const int RefreshIntervalMs = 100;
        private const int DelayIntervalMs = 300;
        private const string FileName = "File";
        private const string NewFileName = "NewFile";
        private const string IntermediateFileName = "IntermediateFile";
        private const string UpdatedIntermediateFileName = "UpdatedIntermediateFile";
        private const string DirectoryPath = "Directory";

        [Fact]
        public async Task TestChangedEvents()
        {
            var fileSystemWatcherWrapperMock = new Mock<IFileSystemWatcher>();
            var pathServiceMock = new Mock<IPathService>();
            var configuration = GetConfiguration();

            var decorator = new AggregatingFileSystemWatcherDecorator(pathServiceMock.Object,
                fileSystemWatcherWrapperMock.Object, configuration);

            var actualCallsCount = 0;
            decorator.Changed += (sender, args) =>
            {
                Assert.Equal(WatcherChangeTypes.Changed, args.ChangeType);
                Assert.Equal(FileName, args.Name);

                actualCallsCount++;
            };

            for (var i = 0; i < 10; i++)
            {
                var changedArgs = new FileSystemEventArgs(WatcherChangeTypes.Changed, DirectoryPath, FileName);
                fileSystemWatcherWrapperMock.Raise(m => m.Changed += null, changedArgs);
            }

            await Task.Delay(DelayIntervalMs);

            Assert.Equal(1, actualCallsCount);
        }

        [Fact]
        public async Task TestChangedAndDeletedEvents()
        {
            var fileSystemWatcherWrapperMock = new Mock<IFileSystemWatcher>();
            var pathServiceMock = new Mock<IPathService>();
            var configuration = GetConfiguration();

            var decorator = new AggregatingFileSystemWatcherDecorator(pathServiceMock.Object,
                fileSystemWatcherWrapperMock.Object, configuration);

            var changedCallsCount = 0;
            var actualCallsCount = 0;
            decorator.Changed += (sender, args) =>
            {
                Assert.Equal(WatcherChangeTypes.Changed, args.ChangeType);
                Assert.Equal(FileName, args.Name);

                changedCallsCount++;
                Assert.Equal(0, actualCallsCount);
            };
            decorator.Deleted += (sender, args) =>
            {
                Assert.Equal(WatcherChangeTypes.Deleted, args.ChangeType);
                Assert.Equal(FileName, args.Name);

                actualCallsCount++;
            };

            var changedArgs = new FileSystemEventArgs(WatcherChangeTypes.Changed, DirectoryPath, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Changed += null, changedArgs);

            var deletedArgs = new FileSystemEventArgs(WatcherChangeTypes.Deleted, DirectoryPath, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Deleted += null, deletedArgs);

            await Task.Delay(DelayIntervalMs);

            Assert.Equal(1, actualCallsCount);
            Assert.True(changedCallsCount <= 1);
        }

        [Fact]
        public async Task TestChangedAndRenamedEvents()
        {
            var fileSystemWatcherWrapperMock = new Mock<IFileSystemWatcher>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetParentDirectory(It.IsAny<string>()))
                .Returns(DirectoryPath);
            var configuration = GetConfiguration();

            var decorator = new AggregatingFileSystemWatcherDecorator(pathServiceMock.Object,
                fileSystemWatcherWrapperMock.Object, configuration);

            var changedCallsCount = 0;
            var actualCallsCount = 0;
            decorator.Changed += (sender, args) =>
            {
                Assert.Equal(WatcherChangeTypes.Changed, args.ChangeType);
                Assert.Equal(FileName, args.Name);
                Assert.Equal(0, actualCallsCount);

                changedCallsCount++;
            };
            decorator.Renamed += (sender, args) =>
            {
                Assert.Equal(WatcherChangeTypes.Renamed, args.ChangeType);
                Assert.Equal(FileName, args.OldName);
                Assert.Equal(NewFileName, args.Name);

                actualCallsCount++;
            };

            var changedArgs = new FileSystemEventArgs(WatcherChangeTypes.Changed, DirectoryPath, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Changed += null, changedArgs);

            var renamedArgs = new RenamedEventArgs(WatcherChangeTypes.Renamed, DirectoryPath, NewFileName, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Renamed += null, renamedArgs);

            await Task.Delay(DelayIntervalMs);

            Assert.Equal(1, actualCallsCount);
            Assert.True(changedCallsCount <= 1);
        }

        [Fact]
        public async Task TestDeletedAndCreatedEvents()
        {
            var fileSystemWatcherWrapperMock = new Mock<IFileSystemWatcher>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetParentDirectory(It.IsAny<string>()))
                .Returns(DirectoryPath);
            var configuration = GetConfiguration();

            var decorator = new AggregatingFileSystemWatcherDecorator(pathServiceMock.Object,
                fileSystemWatcherWrapperMock.Object, configuration);

            var actualCallsCount = 0;
            decorator.Changed += (sender, args) =>
            {
                Assert.Equal(WatcherChangeTypes.Changed, args.ChangeType);
                Assert.Equal(FileName, args.Name);

                actualCallsCount++;
            };

            var deletedArgs = new FileSystemEventArgs(WatcherChangeTypes.Deleted, DirectoryPath, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Deleted += null, deletedArgs);

            var createdArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, DirectoryPath, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Created += null, createdArgs);

            await Task.Delay(DelayIntervalMs);

            Assert.Equal(1, actualCallsCount);
        }

        [Fact]
        public async Task TestRenamedAndDeletedEvents()
        {
            var fileSystemWatcherWrapperMock = new Mock<IFileSystemWatcher>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetParentDirectory(It.IsAny<string>()))
                .Returns(DirectoryPath);
            var configuration = GetConfiguration();

            var decorator = new AggregatingFileSystemWatcherDecorator(pathServiceMock.Object,
                fileSystemWatcherWrapperMock.Object, configuration);

            var actualCallsCount = 0;
            var isCallbackCalled = false;
            decorator.Deleted += (sender, args) =>
            {
                Assert.Equal(WatcherChangeTypes.Deleted, args.ChangeType);
                Assert.Equal(FileName, args.Name);

                actualCallsCount++;
            };
            decorator.Renamed += (sender, args) => isCallbackCalled = true;

            var renamedArgs = new RenamedEventArgs(WatcherChangeTypes.Renamed, DirectoryPath, NewFileName, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Renamed += null, renamedArgs);

            var deletedArgs = new FileSystemEventArgs(WatcherChangeTypes.Deleted, DirectoryPath, NewFileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Deleted += null, deletedArgs);

            await Task.Delay(DelayIntervalMs);

            Assert.Equal(1, actualCallsCount);
            Assert.False(isCallbackCalled);
        }

        [Fact]
        public async Task TestRenamedAndChangedEvents()
        {
            var fileSystemWatcherWrapperMock = new Mock<IFileSystemWatcher>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetParentDirectory(It.IsAny<string>()))
                .Returns(DirectoryPath);
            var configuration = GetConfiguration();

            var decorator = new AggregatingFileSystemWatcherDecorator(pathServiceMock.Object,
                fileSystemWatcherWrapperMock.Object, configuration);

            var actualCallsCount = 0;
            var isCallbackCalled = false;
            decorator.Renamed += (sender, args) =>
            {
                Assert.Equal(WatcherChangeTypes.Renamed, args.ChangeType);
                Assert.Equal(FileName, args.OldName);
                Assert.Equal(NewFileName, args.Name);

                actualCallsCount++;
            };
            decorator.Changed += (sender, args) => isCallbackCalled = true;

            var renamedArgs = new RenamedEventArgs(WatcherChangeTypes.Renamed, DirectoryPath, NewFileName, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Renamed += null, renamedArgs);

            var changedArgs = new FileSystemEventArgs(WatcherChangeTypes.Changed, DirectoryPath, NewFileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Changed += null, changedArgs);

            await Task.Delay(DelayIntervalMs);

            Assert.Equal(1, actualCallsCount);
            Assert.False(isCallbackCalled);
        }

        [Fact]
        public async Task TestRenamedEvents()
        {
            var fileSystemWatcherWrapperMock = new Mock<IFileSystemWatcher>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetParentDirectory(It.IsAny<string>()))
                .Returns(DirectoryPath);
            var configuration = GetConfiguration();

            var decorator = new AggregatingFileSystemWatcherDecorator(pathServiceMock.Object,
                fileSystemWatcherWrapperMock.Object, configuration);

            var actualCallsCount = 0;
            decorator.Renamed += (sender, args) =>
            {
                Assert.Equal(WatcherChangeTypes.Renamed, args.ChangeType);
                Assert.Equal(FileName, args.OldName);
                Assert.Equal(NewFileName, args.Name);

                actualCallsCount++;
            };

            var renamedArgs = new RenamedEventArgs(WatcherChangeTypes.Renamed, DirectoryPath, IntermediateFileName, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Renamed += null, renamedArgs);

            renamedArgs = new RenamedEventArgs(WatcherChangeTypes.Renamed, DirectoryPath, UpdatedIntermediateFileName, IntermediateFileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Renamed += null, renamedArgs);

            renamedArgs = new RenamedEventArgs(WatcherChangeTypes.Renamed, DirectoryPath, NewFileName, UpdatedIntermediateFileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Renamed += null, renamedArgs);

            await Task.Delay(DelayIntervalMs);

            Assert.Equal(1, actualCallsCount);
        }

        [Fact]
        public async Task TestCreatedAndChangedEvents()
        {
            var fileSystemWatcherWrapperMock = new Mock<IFileSystemWatcher>();
            var pathServiceMock = new Mock<IPathService>();
            var configuration = GetConfiguration();

            var decorator = new AggregatingFileSystemWatcherDecorator(pathServiceMock.Object,
                fileSystemWatcherWrapperMock.Object, configuration);

            var actualCallsCount = 0;
            var isCallbackCalled = false;
            decorator.Created += (sender, args) =>
            {
                Assert.Equal(WatcherChangeTypes.Created, args.ChangeType);
                Assert.Equal(FileName, args.Name);

                actualCallsCount++;
            };
            decorator.Changed += (sender, args) => isCallbackCalled = true;

            var createdArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, DirectoryPath, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Created += null, createdArgs);

            var renamedArgs = new FileSystemEventArgs(WatcherChangeTypes.Changed, DirectoryPath, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Changed += null, renamedArgs);

            await Task.Delay(DelayIntervalMs);

            Assert.Equal(1, actualCallsCount);
            Assert.False(isCallbackCalled);
        }

        [Fact]
        public async Task TestCreatedAndDeletedEvents()
        {
            var fileSystemWatcherWrapperMock = new Mock<IFileSystemWatcher>();
            var pathServiceMock = new Mock<IPathService>();
            var configuration = GetConfiguration();

            var decorator = new AggregatingFileSystemWatcherDecorator(pathServiceMock.Object,
                fileSystemWatcherWrapperMock.Object, configuration);

            var isCallbackCalled = false;
            decorator.Created += (sender, args) => isCallbackCalled = true;
            decorator.Deleted += (sender, args) => isCallbackCalled = true;

            var createdArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, DirectoryPath, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Created += null, createdArgs);

            var changedArgs = new FileSystemEventArgs(WatcherChangeTypes.Deleted, DirectoryPath, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Deleted += null, changedArgs);

            await Task.Delay(DelayIntervalMs);

            Assert.False(isCallbackCalled);
        }

        [Fact]
        public async Task TestCreatedAndRenamedEvents()
        {
            var fileSystemWatcherWrapperMock = new Mock<IFileSystemWatcher>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetParentDirectory(It.IsAny<string>()))
                .Returns(DirectoryPath);
            var configuration = GetConfiguration();

            var decorator = new AggregatingFileSystemWatcherDecorator(pathServiceMock.Object,
                fileSystemWatcherWrapperMock.Object, configuration);

            var actualCallsCount = 0;
            var isCallbackCalled = false;
            decorator.Created += (sender, args) =>
            {
                Assert.Equal(WatcherChangeTypes.Created, args.ChangeType);
                Assert.Equal(NewFileName, args.Name);

                actualCallsCount++;
            };
            decorator.Renamed += (sender, args) => isCallbackCalled = false;

            var createdArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, DirectoryPath, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Created += null, createdArgs);

            var renamedArgs = new RenamedEventArgs(WatcherChangeTypes.Renamed, DirectoryPath, NewFileName, FileName);
            fileSystemWatcherWrapperMock.Raise(m => m.Renamed += null, renamedArgs);

            await Task.Delay(DelayIntervalMs);

            Assert.Equal(1, actualCallsCount);
            Assert.False(isCallbackCalled);
        }

        [Fact]
        public async Task TestCleanup()
        {
            var fileSystemWatcherWrapperMock = new Mock<IFileSystemWatcher>();
            fileSystemWatcherWrapperMock
                .Setup(m => m.Dispose())
                .Verifiable();
            fileSystemWatcherWrapperMock
                .Setup(m => m.StopRaisingEvents())
                .Verifiable();
            var pathServiceMock = new Mock<IPathService>();
            var configuration = GetConfiguration();

            var decorator = new AggregatingFileSystemWatcherDecorator(pathServiceMock.Object,
                fileSystemWatcherWrapperMock.Object, configuration);

            var isCallbackCalled = false;
            decorator.Changed += (sender, args) => isCallbackCalled = true;

            decorator.StopRaisingEvents();
            decorator.Dispose();

            for (var i = 0; i < 10; i++)
            {
                var changedArgs = new FileSystemEventArgs(WatcherChangeTypes.Changed, DirectoryPath, FileName);
                fileSystemWatcherWrapperMock.Raise(m => m.Changed += null, changedArgs);
            }

            await Task.Delay(DelayIntervalMs);

            Assert.False(isCallbackCalled);

            fileSystemWatcherWrapperMock.Verify(m => m.StopRaisingEvents(), Times.Once);
            fileSystemWatcherWrapperMock.Verify(m => m.Dispose(), Times.Once);
        }

        private static FileSystemWatcherConfiguration GetConfiguration() => new FileSystemWatcherConfiguration
        {
            RefreshIntervalMs = RefreshIntervalMs
        };
    }
}