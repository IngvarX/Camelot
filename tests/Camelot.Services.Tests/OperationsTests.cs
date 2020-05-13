using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Operations;
using Camelot.TaskPool.Interfaces;
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
                .Setup(m => m.CopyAsync(SourceName, DestinationName))
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

            var callbackCalled = false;
            copyOperation.OperationFinished += (sender, args) => callbackCalled = true;

            await copyOperation.RunAsync();

            Assert.True(callbackCalled);
            filesServiceMock.Verify(m => m.CopyAsync(SourceName, DestinationName), Times.Once());
        }

        [Fact]
        public async Task TestMoveOperation()
        {
         var directoryServiceMock = new Mock<IDirectoryService>();
         var filesServiceMock = new Mock<IFileService>();
         filesServiceMock
             .Setup(m => m.CopyAsync(SourceName, DestinationName))
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

         var callbackCalled = false;
         moveOperation.OperationFinished += (sender, args) => callbackCalled = true;

         await moveOperation.RunAsync();

         Assert.True(callbackCalled);
         filesServiceMock.Verify(m => m.CopyAsync(SourceName, DestinationName), Times.Once());
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
                new UnaryFileSystemOperationSettings(new string[] {}, new[] {SourceName}));

            var callbackCalled = false;
            deleteOperation.OperationFinished += (sender, args) => callbackCalled = true;

            await deleteOperation.RunAsync();

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
                new UnaryFileSystemOperationSettings(new[] {SourceName}, new string[] {}));

            var callbackCalled = false;
            deleteOperation.OperationFinished += (sender, args) => callbackCalled = true;

            await deleteOperation.RunAsync();

            Assert.True(callbackCalled);
            directoryServiceMock.Verify(m => m.RemoveRecursively(SourceName), Times.Once());
        }
    }
}