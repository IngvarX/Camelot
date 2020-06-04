using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Operations;
using Camelot.TaskPool.Interfaces;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class OperationsTests
    {
        private const string SourceName = "Source";
        private const string DestinationName = "Destination";

        private readonly ITaskPool _taskPool;
        private readonly IPathService _pathService;

        public OperationsTests()
        {
            var taskPoolMock = new Mock<ITaskPool>();
            taskPoolMock
                .Setup(m => m.ExecuteAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(x => x());
            _taskPool = taskPoolMock.Object;

            var pathServiceMock = new Mock<IPathService>();
            _pathService = pathServiceMock.Object;
        }

        [Fact]
        public async Task TestCopyOperation()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var filesServiceMock = new Mock<IFileService>();
            filesServiceMock
                .Setup(m => m.CopyAsync(SourceName, DestinationName, false))
                .Verifiable();
            var operationsFactory = new OperationsFactory(
                _taskPool,
                directoryServiceMock.Object,
                filesServiceMock.Object,
                _pathService);
            var settings = new BinaryFileSystemOperationSettings(
                new string[] { },
                new[] {SourceName},
                new string[] { },
                new[] {SourceName},
                new Dictionary<string, string> {[SourceName] = DestinationName}
            );
            var copyOperation = operationsFactory.CreateCopyOperation(settings);

            Assert.Equal(OperationState.NotStarted, copyOperation.State);

            var callbackCalled = false;
            copyOperation.StateChanged += (sender, args) => callbackCalled = true;

            await copyOperation.RunAsync();

            Assert.Equal(OperationState.Finished, copyOperation.State);

            Assert.True(callbackCalled);
            filesServiceMock.Verify(m => m.CopyAsync(SourceName, DestinationName, false), Times.Once());
        }

        [Fact]
        public async Task TestBlockedCopyOperation()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var filesServiceMock = new Mock<IFileService>();
            filesServiceMock
                .Setup(m => m.CopyAsync(SourceName, DestinationName, false))
                .Verifiable();
            filesServiceMock
                .Setup(m => m.CopyAsync(SourceName, DestinationName, true))
                .Verifiable();
            filesServiceMock
                .Setup(m => m.CheckIfExists(DestinationName))
                .Returns(true);
            var operationsFactory = new OperationsFactory(
                _taskPool,
                directoryServiceMock.Object,
                filesServiceMock.Object,
                _pathService);
            var settings = new BinaryFileSystemOperationSettings(
                new string[] { },
                new[] {SourceName},
                new string[] { },
                new[] {SourceName},
                new Dictionary<string, string> {[SourceName] = DestinationName}
            );
            var copyOperation = operationsFactory.CreateCopyOperation(settings);

            var callbackCalled = false;
            copyOperation.StateChanged += async (sender, args) =>
            {
                if (args.OperationState != OperationState.Blocked)
                {
                    return;
                }

                callbackCalled = true;
                filesServiceMock.Verify(m => m.CopyAsync(SourceName, DestinationName, false), Times.Never);

                var operation = (IOperation) sender;
                var (sourceFilePath, _) = operation.BlockedFiles.Single();
                var options = OperationContinuationOptions.CreateContinuationOptions(sourceFilePath, false, OperationContinuationMode.Overwrite);
                await copyOperation.ContinueAsync(options);
            };

            await copyOperation.RunAsync();

            Assert.True(callbackCalled);

            Assert.Equal(OperationState.Finished, copyOperation.State);
            filesServiceMock.Verify(m => m.CopyAsync(SourceName, DestinationName, true), Times.Once);
        }

        [Fact]
        public async Task TestMoveOperation()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var filesServiceMock = new Mock<IFileService>();
            filesServiceMock
             .Setup(m => m.CopyAsync(SourceName, DestinationName, false))
             .Verifiable();
            filesServiceMock
             .Setup(m => m.Remove(SourceName))
             .Verifiable();

            var operationsFactory = new OperationsFactory(
             _taskPool,
             directoryServiceMock.Object,
             filesServiceMock.Object,
             _pathService);
            var settings = new BinaryFileSystemOperationSettings(
             new string[] { },
             new[] {SourceName},
             new string[] { },
             new[] {SourceName},
             new Dictionary<string, string> {[SourceName] = DestinationName}
            );
            var moveOperation = operationsFactory.CreateMoveOperation(settings);

            Assert.Equal(OperationState.NotStarted, moveOperation.State);

            var callbackCalled = false;
            moveOperation.StateChanged += (sender, args) => callbackCalled = true;

            await moveOperation.RunAsync();

            Assert.Equal(OperationState.Finished, moveOperation.State);

            Assert.True(callbackCalled);
            filesServiceMock.Verify(m => m.CopyAsync(SourceName, DestinationName, false), Times.Once());
            filesServiceMock.Verify(m => m.Remove(SourceName), Times.Once());
        }

        [Fact]
        public async Task TestDeleteFileOperation()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var filesServiceMock = new Mock<IFileService>();
            filesServiceMock
                .Setup(m => m.Remove(SourceName))
                .Verifiable();
            var pathServiceMock = new Mock<IPathService>();

            var operationsFactory = new OperationsFactory(
                _taskPool,
                directoryServiceMock.Object,
                filesServiceMock.Object,
                pathServiceMock.Object);
            var deleteOperation = operationsFactory.CreateDeleteOperation(
                new UnaryFileSystemOperationSettings(new string[] {}, new[] {SourceName}, SourceName));

            Assert.Equal(OperationState.NotStarted, deleteOperation.State);
            var callbackCalled = false;
            deleteOperation.StateChanged += (sender, args) => callbackCalled = true;

            await deleteOperation.RunAsync();

            Assert.Equal(OperationState.Finished, deleteOperation.State);

            Assert.True(callbackCalled);
            filesServiceMock.Verify(m => m.Remove(SourceName), Times.Once());
        }

        [Fact]
        public async Task TestDeleteDirectoryOperation()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.RemoveRecursively(SourceName))
                .Verifiable();
            var filesServiceMock = new Mock<IFileService>();
            var pathServiceMock = new Mock<IPathService>();

            var operationsFactory = new OperationsFactory(
                _taskPool,
                directoryServiceMock.Object,
                filesServiceMock.Object,
                pathServiceMock.Object);
            var deleteOperation = operationsFactory.CreateDeleteOperation(
                new UnaryFileSystemOperationSettings(new[] {SourceName}, new string[] {}, SourceName));
            Assert.Equal(OperationState.NotStarted, deleteOperation.State);

            var callbackCalled = false;
            deleteOperation.StateChanged += (sender, args) => callbackCalled = true;

            await deleteOperation.RunAsync();

            Assert.Equal(OperationState.Finished, deleteOperation.State);

            Assert.True(callbackCalled);
            directoryServiceMock.Verify(m => m.RemoveRecursively(SourceName), Times.Once());
        }
    }
}