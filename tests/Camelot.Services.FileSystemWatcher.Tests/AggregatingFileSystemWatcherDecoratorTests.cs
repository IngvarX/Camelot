using System.IO;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.FileSystemWatcher.Configuration;
using Camelot.Services.FileSystemWatcher.Implementations;
using Camelot.Services.FileSystemWatcher.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.FileSystemWatcherWrapper.Tests;

public class AggregatingFileSystemWatcherDecoratorTests
{
    private const int RefreshIntervalMs = 100;
    private const int DelayIntervalMs = 300;
    private const string FileName = "File";
    private const string NewFileName = "NewFile";
    private const string IntermediateFileName = "IntermediateFile";
    private const string UpdatedIntermediateFileName = "UpdatedIntermediateFile";
    private const string DirectoryPath = "Directory";

    private readonly AutoMocker _autoMocker;

    public AggregatingFileSystemWatcherDecoratorTests()
    {
        _autoMocker = new AutoMocker();
        _autoMocker.Use(GetConfiguration());
        _autoMocker
            .Setup<IPathService, string>(m => m.RightTrimPathSeparators(It.IsAny<string>()))
            .Returns<string>(path => path);
    }

    [Fact]
    public async Task TestChangedEvents()
    {
        var decorator = _autoMocker.CreateInstance<AggregatingFileSystemWatcherDecorator>();

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
            _autoMocker.GetMock<IFileSystemWatcher>().Raise(m => m.Changed += null, changedArgs);
        }

        await Task.Delay(DelayIntervalMs);

        Assert.Equal(1, actualCallsCount);
    }

    [Fact]
    public async Task TestChangedAndDeletedEvents()
    {
        var decorator = _autoMocker.CreateInstance<AggregatingFileSystemWatcherDecorator>();

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

        var fileSystemWatcherWrapperMock = _autoMocker.GetMock<IFileSystemWatcher>();

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
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(It.IsAny<string>()))
            .Returns(DirectoryPath);

        var decorator = _autoMocker.CreateInstance<AggregatingFileSystemWatcherDecorator>();

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

        var fileSystemWatcherWrapperMock = _autoMocker.GetMock<IFileSystemWatcher>();

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
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(It.IsAny<string>()))
            .Returns(DirectoryPath);

        var decorator = _autoMocker.CreateInstance<AggregatingFileSystemWatcherDecorator>();

        var actualCallsCount = 0;
        decorator.Changed += (sender, args) =>
        {
            Assert.Equal(WatcherChangeTypes.Changed, args.ChangeType);
            Assert.Equal(FileName, args.Name);

            actualCallsCount++;
        };

        var fileSystemWatcherWrapperMock = _autoMocker.GetMock<IFileSystemWatcher>();

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
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(It.IsAny<string>()))
            .Returns(DirectoryPath);

        var decorator = _autoMocker.CreateInstance<AggregatingFileSystemWatcherDecorator>();

        var actualCallsCount = 0;
        var isCallbackCalled = false;
        decorator.Deleted += (sender, args) =>
        {
            Assert.Equal(WatcherChangeTypes.Deleted, args.ChangeType);
            Assert.Equal(FileName, args.Name);

            actualCallsCount++;
        };
        decorator.Renamed += (sender, args) => isCallbackCalled = true;
        var fileSystemWatcherWrapperMock = _autoMocker.GetMock<IFileSystemWatcher>();

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
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(It.IsAny<string>()))
            .Returns(DirectoryPath);

        var decorator = _autoMocker.CreateInstance<AggregatingFileSystemWatcherDecorator>();

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
        var fileSystemWatcherWrapperMock = _autoMocker.GetMock<IFileSystemWatcher>();

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
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(It.IsAny<string>()))
            .Returns(DirectoryPath);

        var decorator = _autoMocker.CreateInstance<AggregatingFileSystemWatcherDecorator>();

        var actualCallsCount = 0;
        decorator.Renamed += (sender, args) =>
        {
            Assert.Equal(WatcherChangeTypes.Renamed, args.ChangeType);
            Assert.Equal(FileName, args.OldName);
            Assert.Equal(NewFileName, args.Name);

            actualCallsCount++;
        };

        var fileSystemWatcherWrapperMock = _autoMocker.GetMock<IFileSystemWatcher>();

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
        var decorator = _autoMocker.CreateInstance<AggregatingFileSystemWatcherDecorator>();

        var actualCallsCount = 0;
        var isCallbackCalled = false;
        decorator.Created += (sender, args) =>
        {
            Assert.Equal(WatcherChangeTypes.Created, args.ChangeType);
            Assert.Equal(FileName, args.Name);

            actualCallsCount++;
        };
        decorator.Changed += (sender, args) => isCallbackCalled = true;
        var fileSystemWatcherWrapperMock = _autoMocker.GetMock<IFileSystemWatcher>();

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
        var decorator = _autoMocker.CreateInstance<AggregatingFileSystemWatcherDecorator>();

        var isCallbackCalled = false;
        decorator.Created += (sender, args) => isCallbackCalled = true;
        decorator.Deleted += (sender, args) => isCallbackCalled = true;
        var fileSystemWatcherWrapperMock = _autoMocker.GetMock<IFileSystemWatcher>();

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
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(It.IsAny<string>()))
            .Returns(DirectoryPath);

        var decorator = _autoMocker.CreateInstance<AggregatingFileSystemWatcherDecorator>();

        var actualCallsCount = 0;
        var isCallbackCalled = false;
        decorator.Created += (sender, args) =>
        {
            Assert.Equal(WatcherChangeTypes.Created, args.ChangeType);
            Assert.Equal(NewFileName, args.Name);

            actualCallsCount++;
        };
        decorator.Renamed += (sender, args) => isCallbackCalled = false;
        var fileSystemWatcherWrapperMock = _autoMocker.GetMock<IFileSystemWatcher>();

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
        _autoMocker
            .Setup<IFileSystemWatcher>(m => m.Dispose())
            .Verifiable();
        _autoMocker
            .Setup<IFileSystemWatcher>(m => m.StopRaisingEvents())
            .Verifiable();

        var decorator = _autoMocker.CreateInstance<AggregatingFileSystemWatcherDecorator>();

        var isCallbackCalled = false;
        decorator.Changed += (sender, args) => isCallbackCalled = true;

        decorator.StopRaisingEvents();
        decorator.Dispose();

        var fileSystemWatcherWrapperMock = _autoMocker.GetMock<IFileSystemWatcher>();


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