using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
using Camelot.Services.Operations.Interfaces;
using Camelot.Services.Operations.Settings;
using Moq;
using Xunit;

namespace Camelot.Tests
{
    public class OperationsServiceTests
    {
        private const string FileName = "FileName";
        private const string SelectedDirectoryName = "SelectedDirectoryName";
        private const string DirectoryName = "DirectoryName";

        [Fact]
        public void TestEditSelectedFiles()
        {
            var filesSelectionServiceMock = new Mock<IFilesSelectionService>();
            filesSelectionServiceMock
                .SetupGet(m => m.SelectedFiles)
                .Returns(new[] {FileName});

            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileServiceMock = new Mock<IFileService>();
            var fileOpeningServiceMock = new Mock<IFileOpeningService>();
            fileOpeningServiceMock
                .Setup(m => m.Open(FileName))
                .Verifiable();

            IOperationsService operationsService = new OperationsService(
                filesSelectionServiceMock.Object,
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object);

            operationsService.EditSelectedFiles();

            fileOpeningServiceMock.Verify(m => m.Open(FileName), Times.Once());
        }

        [Fact]
        public async Task TestFilesRemoving()
        {
            var filesSelectionServiceMock = new Mock<IFilesSelectionService>();
            filesSelectionServiceMock
                .SetupGet(m => m.SelectedFiles)
                .Returns(new[] {FileName});

            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync(It.IsAny<CancellationToken>()))
                .Verifiable();
            operationsFactoryMock
                .Setup(m => m.CreateDeleteFileOperation(It.IsAny<IList<UnaryFileOperationSettings>>()))
                .Callback<IList<UnaryFileOperationSettings>>(l =>
                {
                    Assert.Equal(FileName, l.Single().FilePath);
                })
                .Returns(operationMock.Object);

            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileOpeningServiceMock = new Mock<IFileOpeningService>();
            var fileServiceMock = new Mock<IFileService>();

            IOperationsService operationsService = new OperationsService(
                filesSelectionServiceMock.Object,
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object);

            await operationsService.RemoveSelectedFilesAsync();

            operationMock.Verify(m => m.RunAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TestFilesMove()
        {
            var filesSelectionServiceMock = new Mock<IFilesSelectionService>();
            filesSelectionServiceMock
                .SetupGet(m => m.SelectedFiles)
                .Returns(new[] {FileName});

            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync(It.IsAny<CancellationToken>()))
                .Verifiable();
            operationsFactoryMock
                .Setup(m => m.CreateMoveOperation(It.IsAny<IList<BinaryFileOperationSettings>>()))
                .Callback<IList<BinaryFileOperationSettings>>(l =>
                {
                    var settings = l.Single();
                    Assert.Equal(FileName, settings.SourceFilePath);
                    Assert.Equal(Path.Combine(DirectoryName, FileName), settings.DestinationFilePath);
                })
                .Returns(operationMock.Object);

            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileOpeningServiceMock = new Mock<IFileOpeningService>();
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfFileExists(FileName))
                .Returns(true);

            IOperationsService operationsService = new OperationsService(
                filesSelectionServiceMock.Object,
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object);

            await operationsService.MoveSelectedFilesAsync(DirectoryName);

            operationMock.Verify(m => m.RunAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TestFilesCopy()
        {
            var filesSelectionServiceMock = new Mock<IFilesSelectionService>();
            filesSelectionServiceMock
                .SetupGet(m => m.SelectedFiles)
                .Returns(new[] {FileName});

            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync(It.IsAny<CancellationToken>()))
                .Verifiable();
            operationsFactoryMock
                .Setup(m => m.CreateCopyOperation(It.IsAny<IList<BinaryFileOperationSettings>>()))
                .Callback<IList<BinaryFileOperationSettings>>(l =>
                {
                    var settings = l.Single();
                    Assert.Equal(FileName, settings.SourceFilePath);
                    Assert.Equal(Path.Combine(DirectoryName, FileName), settings.DestinationFilePath);
                })
                .Returns(operationMock.Object);

            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileOpeningServiceMock = new Mock<IFileOpeningService>();
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfFileExists(FileName))
                .Returns(true);

            IOperationsService operationsService = new OperationsService(
                filesSelectionServiceMock.Object,
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object);

            await operationsService.CopySelectedFilesAsync(DirectoryName);

            operationMock.Verify(m => m.RunAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public void TestDirectoryCreation()
        {
            var filesSelectionServiceMock = new Mock<IFilesSelectionService>();
            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .SetupGet(m => m.SelectedDirectory)
                .Returns(SelectedDirectoryName);
            var fullDirectoryPath = Path.Combine(SelectedDirectoryName, DirectoryName);
            directoryServiceMock
                .Setup(m => m.CreateDirectory(fullDirectoryPath))
                .Verifiable();
            var fileOpeningServiceMock = new Mock<IFileOpeningService>();
            var fileServiceMock = new Mock<IFileService>();

            IOperationsService operationsService = new OperationsService(
                filesSelectionServiceMock.Object,
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object);

            operationsService.CreateDirectory(DirectoryName);

            directoryServiceMock.Verify(m => m.CreateDirectory(fullDirectoryPath), Times.Once());
        }
    }
}