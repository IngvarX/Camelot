using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.TaskPool.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Operations.Tests
{
    public class OperationsTests
    {
        private const string SourceName = "Source";
        private const string SecondSourceName = "SecondSource";
        private const string DestinationName = "Destination";
        private const string SecondDestinationName = "SecondDestination";

        private readonly ITaskPool _taskPool;
        private readonly IPathService _pathService;
        private readonly IFileNameGenerationService _fileNameGenerationService;

        public OperationsTests()
        {
            var taskPoolMock = new Mock<ITaskPool>();
            taskPoolMock
                .Setup(m => m.ExecuteAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(x => x());
            _taskPool = taskPoolMock.Object;

            var pathServiceMock = new Mock<IPathService>();
            _pathService = pathServiceMock.Object;

            var fileNameGenerationServiceMock = new Mock<IFileNameGenerationService>();
            _fileNameGenerationService = fileNameGenerationServiceMock.Object;
        }

        [Theory]
        [InlineData(true, OperationState.Failed)]
        [InlineData(false, OperationState.Finished)]
        public async Task TestCopyOperation(bool throws, OperationState state)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var filesServiceMock = new Mock<IFileService>();
            var copySetup = filesServiceMock
                .Setup(m => m.CopyAsync(SourceName, DestinationName, false));
            if (throws)
            {
                copySetup.ThrowsAsync(new AccessViolationException()).Verifiable();
            }
            else
            {
                copySetup.Verifiable();
            }

            var operationsFactory = new OperationsFactory(
                _taskPool,
                directoryServiceMock.Object,
                filesServiceMock.Object,
                _pathService,
                _fileNameGenerationService);
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
            filesServiceMock.Verify(m => m.CopyAsync(SourceName, DestinationName, false), Times.Once());
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

            var directoryServiceMock = new Mock<IDirectoryService>();
            var filesServiceMock = new Mock<IFileService>();
            filesServiceMock
                .Setup(m => m.GetFile(SourceName))
                .Returns(new FileModel {LastModifiedDateTime = now});
            filesServiceMock
                .Setup(m => m.GetFile(DestinationName))
                .Returns(new FileModel {LastModifiedDateTime = hourBeforeNow});
            filesServiceMock
                .Setup(m => m.GetFile(SecondSourceName))
                .Returns(new FileModel {LastModifiedDateTime = hourBeforeNow});
            filesServiceMock
                .Setup(m => m.GetFile(SecondDestinationName))
                .Returns(new FileModel {LastModifiedDateTime = now});
            filesServiceMock
                .Setup(m => m.CopyAsync(SourceName, DestinationName, false))
                .Verifiable();
            filesServiceMock
                .Setup(m => m.CopyAsync(SourceName, DestinationName, true))
                .Returns(Task.CompletedTask)
                .Verifiable();
            filesServiceMock
                .Setup(m => m.CopyAsync(SecondSourceName, SecondDestinationName, false))
                .Verifiable();
            filesServiceMock
                .Setup(m => m.CopyAsync(SecondSourceName, SecondDestinationName, true))
                .Returns(Task.CompletedTask)
                .Verifiable();
            filesServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns(true);
            var operationsFactory = new OperationsFactory(
                _taskPool,
                directoryServiceMock.Object,
                filesServiceMock.Object,
                _pathService,
                _fileNameGenerationService);
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
            filesServiceMock.Verify(m => m.CopyAsync(SourceName, DestinationName, true), Times.Exactly(expectedWriteCallsCountFirstFile));
            filesServiceMock.Verify(m => m.CopyAsync(SecondSourceName, SecondDestinationName, true), Times.Exactly(expectedWriteCallsCountSecondFile));
            filesServiceMock.Verify(m => m.CopyAsync(SourceName, DestinationName, false), Times.Never);
            filesServiceMock.Verify(m => m.CopyAsync(SecondSourceName, SecondDestinationName, false), Times.Never);
        }

        [Theory]
        [InlineData(true, true, OperationState.Failed)]
        [InlineData(true, false, OperationState.Failed)]
        [InlineData(false, true, OperationState.Failed)]
        [InlineData(false, false, OperationState.Finished)]
        public async Task TestMoveOperation(bool copyThrows, bool deleteThrows, OperationState state)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var filesServiceMock = new Mock<IFileService>();
            var copySetup = filesServiceMock
             .Setup(m => m.CopyAsync(SourceName, DestinationName, false));
            if (copyThrows)
            {
                copySetup.ThrowsAsync(new AccessViolationException()).Verifiable();
            }
            else
            {
                copySetup.Verifiable();
            }

            var deleteSetup = filesServiceMock
                .Setup(m => m.Remove(SourceName));
            if (deleteThrows)
            {
                deleteSetup.Throws(new AccessViolationException()).Verifiable();
            }
            else
            {
                deleteSetup.Verifiable();
            }

            var operationsFactory = new OperationsFactory(
             _taskPool,
             directoryServiceMock.Object,
             filesServiceMock.Object,
             _pathService,
             _fileNameGenerationService);
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
            filesServiceMock.Verify(m => m.CopyAsync(SourceName, DestinationName, false), Times.Once());
            filesServiceMock.Verify(m => m.Remove(SourceName), copyThrows ? Times.Never() : Times.Once());
        }

        [Theory]
        [InlineData(true, OperationState.Failed)]
        [InlineData(false, OperationState.Finished)]
        public async Task TestDeleteFileOperation(bool throws, OperationState state)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var filesServiceMock = new Mock<IFileService>();
            var removeSetup = filesServiceMock
                .Setup(m => m.Remove(SourceName));
            if (throws)
            {
                removeSetup.Throws(new AccessViolationException()).Verifiable();
            }
            else
            {
                removeSetup.Verifiable();
            }
            var pathServiceMock = new Mock<IPathService>();

            var operationsFactory = new OperationsFactory(
                _taskPool,
                directoryServiceMock.Object,
                filesServiceMock.Object,
                pathServiceMock.Object,
                _fileNameGenerationService);
            var deleteOperation = operationsFactory.CreateDeleteOperation(
                new UnaryFileSystemOperationSettings(new string[] {}, new[] {SourceName}, SourceName));

            Assert.Equal(OperationState.NotStarted, deleteOperation.State);
            var callbackCalled = false;
            deleteOperation.StateChanged += (sender, args) => callbackCalled = true;

            await deleteOperation.RunAsync();

            Assert.Equal(state, deleteOperation.State);

            Assert.True(callbackCalled);
            filesServiceMock.Verify(m => m.Remove(SourceName), Times.Once());
        }

        [Theory]
        [InlineData(true, OperationState.Failed)]
        [InlineData(false, OperationState.Finished)]
        public async Task TestDeleteDirectoryOperation(bool throws, OperationState state)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var removeSetup = directoryServiceMock
                .Setup(m => m.RemoveRecursively(SourceName));
            if (throws)
            {
                removeSetup.Throws(new AccessViolationException()).Verifiable();
            }
            else
            {
                removeSetup.Verifiable();
            }
            var filesServiceMock = new Mock<IFileService>();
            var pathServiceMock = new Mock<IPathService>();

            var operationsFactory = new OperationsFactory(
                _taskPool,
                directoryServiceMock.Object,
                filesServiceMock.Object,
                pathServiceMock.Object,
                _fileNameGenerationService);
            var deleteOperation = operationsFactory.CreateDeleteOperation(
                new UnaryFileSystemOperationSettings(new[] {SourceName}, new string[] {}, SourceName));
            Assert.Equal(OperationState.NotStarted, deleteOperation.State);

            var callbackCalled = false;
            deleteOperation.StateChanged += (sender, args) => callbackCalled = true;

            await deleteOperation.RunAsync();

            Assert.Equal(state, deleteOperation.State);

            Assert.True(callbackCalled);
            directoryServiceMock.Verify(m => m.RemoveRecursively(SourceName), Times.Once());
        }

        [Theory]
        [InlineData(false, OperationState.Failed)]
        [InlineData(true, OperationState.Finished)]
        public async Task TestCopeEmptyDirectoryOperation(bool success, OperationState state)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.GetEmptyDirectoriesRecursively(SourceName))
                .Returns(new[] {SourceName});
            directoryServiceMock
                .Setup(m => m.Create(DestinationName))
                .Returns(success)
                .Verifiable();
            var filesServiceMock = new Mock<IFileService>();
            var pathServiceMock = new Mock<IPathService>();

            var operationsFactory = new OperationsFactory(
                _taskPool,
                directoryServiceMock.Object,
                filesServiceMock.Object,
                pathServiceMock.Object,
                _fileNameGenerationService);
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
            directoryServiceMock.Verify(m => m.Create(DestinationName), Times.Once());
        }
    }
}