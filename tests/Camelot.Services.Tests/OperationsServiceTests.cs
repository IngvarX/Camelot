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

namespace Camelot.Services.Tests
{
    public class OperationsServiceTests
    {
        private const string FileName = "FileName";
        private const string SelectedDirectoryName = "SelectedDirectoryName";
        private const string DirectoryName = "DirectoryName";

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

        [Fact]
        public void TestEditSelectedFiles()
        {
            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileServiceMock = new Mock<IFileService>();
            var fileOpeningServiceMock = new Mock<IResourceOpeningService>();
            fileOpeningServiceMock
                .Setup(m => m.Open(FileName))
                .Verifiable();
            var pathServiceMock = new Mock<IPathService>();
            var driveServiceMock = new Mock<IDriveService>();
            var trashCanLocatorMock = new Mock<ITrashCanLocator>();

            IOperationsService operationsService = new OperationsService(
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object,
                pathServiceMock.Object,
                driveServiceMock.Object,
                trashCanLocatorMock.Object);

            operationsService.EditFiles(new[] {FileName});

            fileOpeningServiceMock.Verify(m => m.Open(FileName), Times.Once());
        }

        [Fact]
        public async Task TestFilesRemoving()
        {
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
            var fileOpeningServiceMock = new Mock<IResourceOpeningService>();
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfExists(FileName))
                .Returns(true)
                .Verifiable();
            var pathServiceMock = new Mock<IPathService>();
            var driveServiceMock = new Mock<IDriveService>();
            var trashCanLocatorMock = new Mock<ITrashCanLocator>();

            IOperationsService operationsService = new OperationsService(
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object,
                pathServiceMock.Object,
                driveServiceMock.Object,
                trashCanLocatorMock.Object);

            await operationsService.RemoveFilesAsync(new[] {FileName});

            operationMock.Verify(m => m.RunAsync(It.IsAny<CancellationToken>()), Times.Once());
            fileServiceMock.Verify(m => m.CheckIfExists(FileName), Times.Once());
        }

        [Fact]
        public async Task TestFilesMove()
        {
            var fullPath = Path.Combine(DirectoryName, FileName);
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
                    Assert.Equal(fullPath, settings.DestinationFilePath);
                })
                .Returns(operationMock.Object);

            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .SetupGet(m => m.SelectedDirectory)
                .Returns(CurrentDirectory);
            var fileOpeningServiceMock = new Mock<IResourceOpeningService>();
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfExists(FileName))
                .Returns(true);
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetCommonRootDirectory(It.IsAny<IList<string>>()))
                .Returns(string.Empty);
            pathServiceMock
                .Setup(m => m.Combine(DirectoryName, FileName))
                .Returns(fullPath);
            pathServiceMock
                .Setup(m => m.GetRelativePath(string.Empty, FileName))
                .Returns(FileName);
            var driveServiceMock = new Mock<IDriveService>();
            var trashCanLocatorMock = new Mock<ITrashCanLocator>();

            IOperationsService operationsService = new OperationsService(
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object,
                pathServiceMock.Object,
                driveServiceMock.Object,
                trashCanLocatorMock.Object);

            await operationsService.MoveFilesAsync(new[] {FileName}, DirectoryName);

            operationMock.Verify(m => m.RunAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task TestFilesCopy()
        {
            var fullPath = Path.Combine(DirectoryName, FileName);
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
                    Assert.Equal(fullPath, settings.DestinationFilePath);
                })
                .Returns(operationMock.Object);

            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .SetupGet(m => m.SelectedDirectory)
                .Returns(CurrentDirectory);
            var fileOpeningServiceMock = new Mock<IResourceOpeningService>();
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfExists(FileName))
                .Returns(true);
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetCommonRootDirectory(It.IsAny<IList<string>>()))
                .Returns(string.Empty);
            pathServiceMock
                .Setup(m => m.Combine(DirectoryName, FileName))
                .Returns(fullPath);
            pathServiceMock
                .Setup(m => m.GetRelativePath(string.Empty, FileName))
                .Returns(FileName);
            var driveServiceMock = new Mock<IDriveService>();
            var trashCanLocatorMock = new Mock<ITrashCanLocator>();

            IOperationsService operationsService = new OperationsService(
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object,
                pathServiceMock.Object,
                driveServiceMock.Object,
                trashCanLocatorMock.Object);

            await operationsService.CopyFilesAsync(new[] {FileName}, DirectoryName);

            operationMock.Verify(m => m.RunAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public void TestDirectoryCreation()
        {
            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var directoryServiceMock = new Mock<IDirectoryService>();
            var fullDirectoryPath = Path.Combine(SelectedDirectoryName, DirectoryName);
            directoryServiceMock
                .Setup(m => m.Create(fullDirectoryPath))
                .Verifiable();
            var fileOpeningServiceMock = new Mock<IResourceOpeningService>();
            var fileServiceMock = new Mock<IFileService>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.Combine(SelectedDirectoryName, DirectoryName))
                .Returns(fullDirectoryPath);
            var driveServiceMock = new Mock<IDriveService>();
            var trashCanLocatorMock = new Mock<ITrashCanLocator>();

            IOperationsService operationsService = new OperationsService(
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object,
                pathServiceMock.Object,
                driveServiceMock.Object,
                trashCanLocatorMock.Object);

            operationsService.CreateDirectory(SelectedDirectoryName, DirectoryName);

            directoryServiceMock.Verify(m => m.Create(fullDirectoryPath), Times.Once());
        }
    }
}