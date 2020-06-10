using System.IO;
using System.Threading.Tasks;
using Camelot.FileSystemWatcherWrapper.Configuration;
using Camelot.FileSystemWatcherWrapper.Implementations;
using Camelot.FileSystemWatcherWrapper.Interfaces;
using Camelot.Services.Abstractions;
using Moq;
using Xunit;

namespace Camelot.FileSystemWatcherWrapper.Tests
{
    public class AggregatingFileSystemWatcherDecoratorTests
    {
        private const int RefreshIntervalMs = 1_000;
        private const int DelayIntervalMs = 5 * RefreshIntervalMs;
        private const string FileName = "File";
        private const string NewFileName = "NewFile";
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

            for (var i = 0; i < 100; i++)
            {
                var changedArgs = new FileSystemEventArgs(WatcherChangeTypes.Changed, DirectoryPath, FileName);
                fileSystemWatcherWrapperMock.Raise(m => m.Changed += null, changedArgs);
            }

            await Task.Delay(DelayIntervalMs);

            Assert.Equal(1, actualCallsCount);
        }

        [Fact]
        public async Task TestChangedAndDeleteEvents()
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

                changedCallsCount++;
                Assert.Equal(0, actualCallsCount);
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

        private static FileSystemWatcherConfiguration GetConfiguration() => new FileSystemWatcherConfiguration
        {
            RefreshIntervalMs = RefreshIntervalMs
        };
    }
}