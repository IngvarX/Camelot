using System;
using System.IO;
using System.Threading.Tasks;
using Camelot.Services.Interfaces;
using Camelot.Services.Operations.Implementations;
using Camelot.Services.Operations.Settings;
using Camelot.TaskPool.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class OperationsTests
    {
        private const string SourceFileName = "Source";
        private const string DestinationFileName = "Destination";

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

        private static string SourceFile => Path.Combine(CurrentDirectory, SourceFileName);

        private static string DestinationFile => Path.Combine(CurrentDirectory, DestinationFileName);

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
                .Setup(m => m.CopyFileAsync(SourceFile, DestinationFile))
                .Verifiable();
            var operationsFactory = new OperationsFactory(
                _taskPool,
                directoryServiceMock.Object,
                filesServiceMock.Object,
                _pathService);
            var copyOperation = operationsFactory.CreateCopyOperation(
                new[]
                {
                    new BinaryFileOperationSettings(SourceFile, DestinationFile)
                });

            var callbackCalled = false;
            copyOperation.OperationFinished += (sender, args) => callbackCalled = true;

            await copyOperation.RunAsync();

            Assert.True(callbackCalled);
            filesServiceMock.Verify(m => m.CopyFileAsync(SourceFile, DestinationFile), Times.Once());
        }

        [Fact]
        public async Task TestMoveOperation()
        {
         var directoryServiceMock = new Mock<IDirectoryService>();
         var filesServiceMock = new Mock<IFileService>();
         filesServiceMock
             .Setup(m => m.CopyFileAsync(SourceFile, DestinationFile))
             .Verifiable();
         filesServiceMock
             .Setup(m => m.RemoveFile(SourceFile))
             .Verifiable();

         var operationsFactory = new OperationsFactory(
             _taskPool,
             directoryServiceMock.Object,
             filesServiceMock.Object,
             _pathService);
         var moveOperation = operationsFactory.CreateMoveOperation(
             new[]
             {
                 new BinaryFileOperationSettings(SourceFile, DestinationFile)
             });

         var callbackCalled = false;
         moveOperation.OperationFinished += (sender, args) => callbackCalled = true;

         await moveOperation.RunAsync();

         Assert.True(callbackCalled);
         filesServiceMock.Verify(m => m.CopyFileAsync(SourceFile, DestinationFile), Times.Once());
         filesServiceMock.Verify(m => m.RemoveFile(SourceFile), Times.Once());
        }

        [Fact]
        public async Task TestDeleteOperation()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var filesServiceMock = new Mock<IFileService>();
            filesServiceMock
                .Setup(m => m.RemoveFile(SourceFile))
                .Verifiable();
            var pathServiceMock = new Mock<IPathService>();

            var operationsFactory = new OperationsFactory(
                _taskPool,
                directoryServiceMock.Object,
                filesServiceMock.Object,
                pathServiceMock.Object);
            var deleteOperation = operationsFactory.CreateDeleteFileOperation(
                new[]
                {
                    new UnaryFileOperationSettings(SourceFile),
                });

            var callbackCalled = false;
            deleteOperation.OperationFinished += (sender, args) => callbackCalled = true;

            await deleteOperation.RunAsync();

            Assert.True(callbackCalled);
            filesServiceMock.Verify(m => m.RemoveFile(SourceFile), Times.Once());
        }
    }
}