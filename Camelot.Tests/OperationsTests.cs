using System;
using System.IO;
using System.Threading.Tasks;
using Camelot.Services.Operations.Implementations;
using Camelot.Services.Operations.Interfaces;
using Camelot.Services.Operations.Settings;
using Camelot.TaskPool.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Tests
{
    public class OperationsTests : IDisposable
    {
        private const string SourceFileName = "Source";
        private const string DestinationFileName = "Destination";

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

        private string SourceFile => Path.Combine(CurrentDirectory, SourceFileName);

        private string DestinationFile => Path.Combine(CurrentDirectory, DestinationFileName);

        private readonly IOperationsFactory _operationsFactory;

        public OperationsTests()
        {
            var taskPoolMock = new Mock<ITaskPool>();
            taskPoolMock
                .Setup(m => m.ExecuteAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(x => x());

            _operationsFactory = new OperationsFactory(taskPoolMock.Object);

            CreateSourceFile();
        }

        [Fact]
        public async Task TestCopyOperation()
        {
            var copyOperation = _operationsFactory.CreateCopyOperation(
                new[]
                {
                    new BinaryFileOperationSettings(SourceFile, DestinationFile)
                });

            var callbackCalled = false;
            copyOperation.OperationFinished += (sender, args) => callbackCalled = true;

            await copyOperation.RunAsync();

            Assert.True(File.Exists(SourceFileName));
            Assert.True(File.Exists(DestinationFile));
            Assert.True(callbackCalled);
        }

        [Fact]
        public async Task TestMoveOperation()
        {
            var moveOperation = _operationsFactory.CreateMoveOperation(
                new[]
                {
                    new BinaryFileOperationSettings(SourceFile, DestinationFile)
                });

            var callbackCalled = false;
            moveOperation.OperationFinished += (sender, args) => callbackCalled = true;

            await moveOperation.RunAsync();

            Assert.False(File.Exists(SourceFileName));
            Assert.True(File.Exists(DestinationFile));
            Assert.True(callbackCalled);
        }

        [Fact]
        public async Task TestDeleteOperation()
        {
            var deleteOperation = _operationsFactory.CreateDeleteOperation(
                new[]
                {
                    new UnaryFileOperationSettings(SourceFile),
                });

            var callbackCalled = false;
            deleteOperation.OperationFinished += (sender, args) => callbackCalled = true;

            await deleteOperation.RunAsync();

            Assert.False(File.Exists(SourceFileName));
            Assert.True(callbackCalled);
        }

        public void Dispose()
        {
            if (File.Exists(SourceFile))
            {
                File.Delete(SourceFile);
            }

            if (File.Exists(DestinationFile))
            {
                File.Delete(DestinationFile);
            }
        }

        private void CreateSourceFile()
        {
            var file = File.Create(SourceFile);
            file.Close();
        }
    }
}