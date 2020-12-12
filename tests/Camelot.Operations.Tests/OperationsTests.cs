using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Operations.Tests
{
    public class OperationsTests
    {
        private const string SourceName = "Source";
        private const string SecondSourceName = "SecondSource";
        private const string DestinationName = "Destination";
        private const string SourceDirName = "SourceDir";
        private const string DestinationDirName = "DestinationDir";
        private const string SecondDestinationName = "SecondDestination";

        private readonly AutoMocker _autoMocker;

        public OperationsTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(true, OperationState.Failed)]
        [InlineData(false, OperationState.Finished)]
        public async Task TestCopyOperation(bool throws, OperationState state)
        {
            var copySetup = _autoMocker
                .Setup<IFileService, Task<bool>>(m => m.CopyAsync(SourceName, DestinationName, false))
                .ReturnsAsync(!throws);
            copySetup.Verifiable();

            var operationsFactory = _autoMocker.CreateInstance<OperationsFactory>();
            var settings = new BinaryFileSystemOperationSettings(
                new string[] { },
                new[] {SourceName},
                new string[] { },
                new[] {SourceName},
                new Dictionary<string, string> {[SourceName] = DestinationName},
                new string[] { }
            );
            var copyOperation = operationsFactory.CreateCopyOperation(settings);

            Assert.Equal(OperationState.NotStarted, copyOperation.State);

            var isCallbackCalled = false;
            copyOperation.StateChanged += (sender, args) => isCallbackCalled = true;

            await copyOperation.RunAsync();

            Assert.Equal(state, copyOperation.State);

            Assert.True(isCallbackCalled);
            _autoMocker.Verify<IFileService>(m => m.CopyAsync(SourceName, DestinationName, false), Times.Once);
        }

        [Theory]
        [InlineData(true, 1, OperationContinuationMode.Overwrite, 1, 1)]
        [InlineData(false, 2, OperationContinuationMode.Overwrite, 1, 1)]
        [InlineData(true, 1, OperationContinuationMode.Skip, 0, 0)]
        [InlineData(false, 2, OperationContinuationMode.Skip, 0, 0)]
        [InlineData(true, 1, OperationContinuationMode.OverwriteIfOlder, 1, 0)]
        [InlineData(false, 2, OperationContinuationMode.OverwriteIfOlder, 1, 0)]
        public async Task TestBlockedCopyOperation(bool applyToAll, int expectedCallbackCallsCount,
            OperationContinuationMode mode, int expectedWriteCallsCountFirstFile, int expectedWriteCallsCountSecondFile)
        {
            var now = DateTime.UtcNow;
            var hourBeforeNow = now.AddHours(-1);

            _autoMocker
                .Setup<IFileService, FileModel>(m => m.GetFile(SourceName))
                .Returns(new FileModel {LastModifiedDateTime = now});
            _autoMocker
                .Setup<IFileService, FileModel>(m => m.GetFile(DestinationName))
                .Returns(new FileModel {LastModifiedDateTime = hourBeforeNow});
            _autoMocker
                .Setup<IFileService, FileModel>(m => m.GetFile(SecondSourceName))
                .Returns(new FileModel {LastModifiedDateTime = hourBeforeNow});
            _autoMocker
                .Setup<IFileService, FileModel>(m => m.GetFile(SecondDestinationName))
                .Returns(new FileModel {LastModifiedDateTime = now});
            _autoMocker
                .Setup<IFileService>(m => m.CopyAsync(SourceName, DestinationName, false))
                .Verifiable();
            _autoMocker
                .Setup<IFileService, Task<bool>>(m => m.CopyAsync(SourceName, DestinationName, true))
                .ReturnsAsync(true)
                .Verifiable();
            _autoMocker
                .Setup<IFileService>(m => m.CopyAsync(SecondSourceName, SecondDestinationName, false))
                .Verifiable();
            _autoMocker
                .Setup<IFileService, Task<bool>>(m => m.CopyAsync(SecondSourceName, SecondDestinationName, true))
                .ReturnsAsync(true)
                .Verifiable();
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns(true);
            var operationsFactory = _autoMocker.CreateInstance<OperationsFactory>();
            var settings = new BinaryFileSystemOperationSettings(
                new string[] { },
                new[] {SourceName, SecondSourceName},
                new string[] { },
                new[] {DestinationName, SecondDestinationName},
                new Dictionary<string, string>
                {
                    [SourceName] = DestinationName,
                    [SecondSourceName] = SecondDestinationName
                },
                new string[] { }
            );
            var copyOperation = operationsFactory.CreateCopyOperation(settings);

            var callbackCallsCount = 0;
            copyOperation.StateChanged += async (sender, args) =>
            {
                if (args.OperationState != OperationState.Blocked)
                {
                    return;
                }

                var operation = (IOperation) sender;
                if (operation is null)
                {
                    return;
                }

                callbackCallsCount++;

                var (sourceFilePath, _) = operation.CurrentBlockedFile;
                var options = OperationContinuationOptions.CreateContinuationOptions(sourceFilePath, applyToAll, mode);

                await copyOperation.ContinueAsync(options);
            };

            await copyOperation.RunAsync();

            Assert.Equal(expectedCallbackCallsCount, callbackCallsCount);

            Assert.Equal(OperationState.Finished, copyOperation.State);
            _autoMocker
                .Verify<IFileService>(m => m.CopyAsync(SourceName, DestinationName, true), Times.Exactly(expectedWriteCallsCountFirstFile));
            _autoMocker
                .Verify<IFileService>(m => m.CopyAsync(SecondSourceName, SecondDestinationName, true), Times.Exactly(expectedWriteCallsCountSecondFile));
            _autoMocker
                .Verify<IFileService>(m => m.CopyAsync(SourceName, DestinationName, false), Times.Never);
            _autoMocker
                .Verify<IFileService>(m => m.CopyAsync(SecondSourceName, SecondDestinationName, false), Times.Never);
        }

        [Theory]
        [InlineData(true, true, OperationState.Failed)]
        [InlineData(true, false, OperationState.Failed)]
        [InlineData(false, true, OperationState.Failed)]
        [InlineData(false, false, OperationState.Finished)]
        public async Task TestMoveOperation(bool copyThrows, bool deleteThrows, OperationState state)
        {
            var copySetup = _autoMocker
                .Setup<IFileService, Task<bool>>(m => m.CopyAsync(SourceName, DestinationName, false))
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
                .Verify<IFileService>(m => m.CopyAsync(SourceName, DestinationName, false), Times.Once());
            _autoMocker
                .Verify<IFileService, bool>(m => m.Remove(SourceName), copyThrows ? Times.Never() : Times.Once());
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
        public async Task TestCopeEmptyDirectoryOperation(bool success, OperationState state)
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
}