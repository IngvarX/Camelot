using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Operations.Tests;

public class OperationsTests
{
    private const string SourceName = "Source";
    private const string DestinationName = "Destination";
    private const string SourceDirName = "SourceDir";
    private const string DestinationDirName = "DestinationDir";

    private readonly AutoMocker _autoMocker;

    public OperationsTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData(true, OperationState.Failed, SourceName, DestinationName, 1, false)]
    [InlineData(false, OperationState.Finished, SourceName, DestinationName, 1, false)]
    [InlineData(false, OperationState.Finished, SourceName, SourceName, 0, true)]
    public async Task TestCopyOperation(bool throws, OperationState state, string sourceName,
        string destinationName, int copyCallsCount, bool destinationExists)
    {
        var copySetup = _autoMocker
            .Setup<IFileService, Task<bool>>(m => m.CopyAsync(sourceName, destinationName, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync(!throws);
        copySetup.Verifiable();
        _autoMocker
            .Setup<IFileService, bool>(m => m.CheckIfExists(destinationName))
            .Returns(destinationExists);

        var operationsFactory = _autoMocker.CreateInstance<OperationsFactory>();
        var settings = new BinaryFileSystemOperationSettings(
            new string[] { },
            new[] {SourceName},
            new string[] { },
            new[] {SourceName},
            new Dictionary<string, string> {[sourceName] = destinationName},
            new string[] { }
        );
        var copyOperation = operationsFactory.CreateCopyOperation(settings);

        Assert.Equal(OperationState.NotStarted, copyOperation.State);

        var isCallbackCalled = false;
        copyOperation.StateChanged += (sender, args) => isCallbackCalled = true;

        await copyOperation.RunAsync();

        Assert.Equal(state, copyOperation.State);

        Assert.True(isCallbackCalled);
        _autoMocker
            .Verify<IFileService>(m => m.CopyAsync(sourceName, destinationName, It.IsAny<CancellationToken>(), false),
                Times.Exactly(copyCallsCount));
    }

    [Theory]
    [InlineData(true, true, OperationState.Failed, 0)]
    [InlineData(true, false, OperationState.Failed, 0)]
    [InlineData(false, true, OperationState.Failed, 1)]
    [InlineData(false, false, OperationState.Finished, 1)]
    public async Task TestMoveOperation(bool copyThrows, bool deleteThrows, OperationState state,
        int removeCallsCount)
    {
        var copySetup = _autoMocker
            .Setup<IFileService, Task<bool>>(m => m.CopyAsync(SourceName, DestinationName, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync(!copyThrows);
        copySetup.Verifiable();

        var deleteSetup = _autoMocker
            .Setup<IFileService, bool>(m => m.Remove(SourceName))
            .Returns(!deleteThrows);
        deleteSetup.Verifiable();

        var operationsFactory = _autoMocker.CreateInstance<OperationsFactory>();
        var settings = new BinaryFileSystemOperationSettings(
            new string[] { },
            new[] {SourceName},
            new string[] { },
            new[] {SourceName},
            new Dictionary<string, string> {[SourceName] = DestinationName},
            new string[] { }
        );
        var moveOperation = operationsFactory.CreateMoveOperation(settings);

        Assert.Equal(OperationState.NotStarted, moveOperation.State);

        var callbackCalled = false;
        moveOperation.StateChanged += (sender, args) => callbackCalled = true;

        await moveOperation.RunAsync();

        Assert.Equal(state, moveOperation.State);

        Assert.True(callbackCalled);
        _autoMocker
            .Verify<IFileService>(m => m.CopyAsync(SourceName, DestinationName, It.IsAny<CancellationToken>(), false),
                Times.Once);
        _autoMocker
            .Verify<IFileService, bool>(m => m.Remove(SourceName),
                Times.Exactly(removeCallsCount));
    }

    [Theory]
    [InlineData(true, OperationState.Failed)]
    [InlineData(false, OperationState.Finished)]
    public async Task TestDeleteFileOperation(bool throws, OperationState state)
    {
        var removeSetup = _autoMocker
            .Setup<IFileService, bool>(m => m.Remove(SourceName))
            .Returns(!throws);
        removeSetup.Verifiable();

        var operationsFactory = _autoMocker.CreateInstance<OperationsFactory>();
        var deleteOperation = operationsFactory.CreateDeleteOperation(
            new UnaryFileSystemOperationSettings(new string[] {}, new[] {SourceName}, SourceName));

        Assert.Equal(OperationState.NotStarted, deleteOperation.State);
        var callbackCalled = false;
        deleteOperation.StateChanged += (sender, args) => callbackCalled = true;

        await deleteOperation.RunAsync();

        Assert.Equal(state, deleteOperation.State);

        Assert.True(callbackCalled);
        _autoMocker.Verify<IFileService, bool>(m => m.Remove(SourceName), Times.Once);
    }

    [Theory]
    [InlineData(true, OperationState.Failed)]
    [InlineData(false, OperationState.Finished)]
    public async Task TestDeleteDirectoryOperation(bool throws, OperationState state)
    {
        var removeSetup = _autoMocker
            .Setup<IDirectoryService, bool>(m => m.RemoveRecursively(SourceName))
            .Returns(!throws);
        removeSetup.Verifiable();

        var operationsFactory = _autoMocker.CreateInstance<OperationsFactory>();
        var deleteOperation = operationsFactory.CreateDeleteOperation(
            new UnaryFileSystemOperationSettings(new[] {SourceName}, new string[] {}, SourceName));
        Assert.Equal(OperationState.NotStarted, deleteOperation.State);

        var callbackCalled = false;
        deleteOperation.StateChanged += (sender, args) => callbackCalled = true;

        await deleteOperation.RunAsync();

        Assert.Equal(state, deleteOperation.State);

        Assert.True(callbackCalled);
        _autoMocker.Verify<IDirectoryService, bool>(m => m.RemoveRecursively(SourceName), Times.Once);
    }

    [Theory]
    [InlineData(false, OperationState.Failed)]
    [InlineData(true, OperationState.Finished)]
    public async Task TestCopyEmptyDirectoryOperation(bool success, OperationState state)
    {
        _autoMocker
            .Setup<IDirectoryService, IReadOnlyList<string>>(m => m.GetEmptyDirectoriesRecursively(SourceName))
            .Returns(new[] {SourceName});
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.Create(DestinationName))
            .Returns(success)
            .Verifiable();

        var operationsFactory = _autoMocker.CreateInstance<OperationsFactory>();
        var settings = new BinaryFileSystemOperationSettings(
            new[] { SourceName },
            new string[] { },
            new[] { DestinationName },
            new string[] { },
            new Dictionary<string, string>(),
            new[] {DestinationName }
        );
        var copyOperation = operationsFactory.CreateCopyOperation(settings);
        Assert.Equal(OperationState.NotStarted, copyOperation.State);

        var callbackCalled = false;
        copyOperation.StateChanged += (sender, args) => callbackCalled = true;

        await copyOperation.RunAsync();

        Assert.Equal(state, copyOperation.State);

        Assert.True(callbackCalled);
        _autoMocker.Verify<IDirectoryService, bool>(m => m.Create(DestinationName), Times.Once);
    }

    [Theory]
    [InlineData(ArchiveType.Tar)]
    [InlineData(ArchiveType.Zip)]
    [InlineData(ArchiveType.Gz)]
    [InlineData(ArchiveType.TarBz2)]
    [InlineData(ArchiveType.TarGz)]
    [InlineData(ArchiveType.Bz2)]
    [InlineData(ArchiveType.TarXz)]
    [InlineData(ArchiveType.Xz)]
    [InlineData(ArchiveType.TarLz)]
    [InlineData(ArchiveType.Lz)]
    [InlineData(ArchiveType.SevenZip)]
    public async Task TestPackOperation(ArchiveType archiveType)
    {
        var processorMock = new Mock<IArchiveWriter>();
        processorMock
            .Setup(m => m.PackAsync(
                It.Is<IReadOnlyList<string>>(l => l.Single() == SourceName),
                It.IsAny<IReadOnlyList<string>>(), SourceDirName, DestinationName))
            .Verifiable();
        _autoMocker
            .Setup<IArchiveProcessorFactory, IArchiveWriter>(m => m.CreateWriter(archiveType))
            .Returns(processorMock.Object);

        var operationsFactory = _autoMocker.CreateInstance<OperationsFactory>();
        var settings = new PackOperationSettings(
            new string[] {}, new[] {SourceName},
            DestinationName, SourceDirName, DestinationDirName, archiveType);
        var operation = operationsFactory.CreatePackOperation(settings);

        Assert.Equal(OperationState.NotStarted, operation.State);
        var callbackCalled = false;
        operation.StateChanged += (sender, args) => callbackCalled = true;

        await operation.RunAsync();

        Assert.Equal(OperationState.Finished, operation.State);
        Assert.True(callbackCalled);

        processorMock
            .Verify(m => m.PackAsync(
                    It.Is<IReadOnlyList<string>>(l => l.Single() == SourceName),
                    It.IsAny<IReadOnlyList<string>>(), SourceDirName, DestinationName),
                Times.Once);
    }

    [Theory]
    [InlineData(ArchiveType.Tar)]
    [InlineData(ArchiveType.Zip)]
    [InlineData(ArchiveType.Gz)]
    [InlineData(ArchiveType.TarBz2)]
    [InlineData(ArchiveType.TarGz)]
    [InlineData(ArchiveType.Bz2)]
    [InlineData(ArchiveType.TarXz)]
    [InlineData(ArchiveType.Xz)]
    [InlineData(ArchiveType.TarLz)]
    [InlineData(ArchiveType.Lz)]
    [InlineData(ArchiveType.SevenZip)]
    public async Task TestExtractOperation(ArchiveType archiveType)
    {
        var processorMock = new Mock<IArchiveReader>();
        processorMock
            .Setup(m => m.ExtractAsync(
                SourceName, DestinationDirName))
            .Verifiable();
        _autoMocker
            .Setup<IArchiveProcessorFactory, IArchiveReader>(m => m.CreateReader(archiveType))
            .Returns(processorMock.Object);

        var operationsFactory = _autoMocker.CreateInstance<OperationsFactory>();
        var settings = new ExtractArchiveOperationSettings(
            SourceName, DestinationDirName, archiveType);
        var operation = operationsFactory.CreateExtractOperation(settings);

        Assert.Equal(OperationState.NotStarted, operation.State);
        var callbackCalled = false;
        operation.StateChanged += (sender, args) => callbackCalled = true;

        await operation.RunAsync();

        Assert.Equal(OperationState.Finished, operation.State);
        Assert.True(callbackCalled);

        processorMock
            .Verify(m => m.ExtractAsync(
                SourceName, DestinationDirName), Times.Once);
    }
}