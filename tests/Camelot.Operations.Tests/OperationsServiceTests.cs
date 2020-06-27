using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Operations;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Moq;
using Xunit;

namespace Camelot.Services.Operations.Tests
{
    public class OperationsServiceTests
    {
        private const string FileName = "FileName";
        private const string NewFileName = "NewFileName";
        private const string SelectedDirectoryName = "SelectedDirectoryName";
        private const string DirectoryName = "DirectoryName";
        private const string NewDirectoryName = "NewDirectoryName";

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

        [Fact]
        public void TestEditSelectedFiles()
        {
            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileServiceMock = new Mock<IFileService>();
            var fileOpeningServiceMock = new Mock<IResourceOpeningService>();
            var fileOperationsStateServiceMock = new Mock<IOperationsStateService>();
            fileOpeningServiceMock
                .Setup(m => m.Open(FileName))
                .Verifiable();
            var pathServiceMock = new Mock<IPathService>();

            IOperationsService operationsService = new OperationsService(
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object,
                pathServiceMock.Object,
                fileOperationsStateServiceMock.Object);

            operationsService.OpenFiles(new[] {FileName});

            fileOpeningServiceMock.Verify(m => m.Open(FileName), Times.Once());
        }

        [Fact]
        public async Task TestFilesRemoving()
        {
            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync())
                .Verifiable();
            operationsFactoryMock
                .Setup(m => m.CreateDeleteOperation(It.IsAny<UnaryFileSystemOperationSettings>()))
                .Callback<UnaryFileSystemOperationSettings>(s =>
                {
                    Assert.Equal(FileName, s.TopLevelFiles.Single());
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
            var fileOperationsStateServiceMock = new Mock<IOperationsStateService>();

            IOperationsService operationsService = new OperationsService(
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object,
                pathServiceMock.Object,
                fileOperationsStateServiceMock.Object);

            await operationsService.RemoveAsync(new[] {FileName});

            operationMock.Verify(m => m.RunAsync(), Times.Once());
            fileServiceMock.Verify(m => m.CheckIfExists(FileName), Times.Once());
        }

        [Fact]
        public async Task TestFilesMove()
        {
            var fullPath = Path.Combine(DirectoryName, FileName);
            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync())
                .Verifiable();
            operationsFactoryMock
                .Setup(m => m.CreateMoveOperation(It.IsAny<BinaryFileSystemOperationSettings>()))
                .Callback<BinaryFileSystemOperationSettings>(s =>
                {
                    var (key, value) = s.FilesDictionary.Single();

                    Assert.Equal(FileName, key);
                    Assert.Equal(fullPath, value);
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
            var fileOperationsStateServiceMock = new Mock<IOperationsStateService>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetCommonRootDirectory(It.IsAny<IReadOnlyList<string>>()))
                .Returns(string.Empty);
            pathServiceMock
                .Setup(m => m.Combine(DirectoryName, FileName))
                .Returns(fullPath);
            pathServiceMock
                .Setup(m => m.GetRelativePath(string.Empty, FileName))
                .Returns(FileName);

            IOperationsService operationsService = new OperationsService(
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object,
                pathServiceMock.Object,
                fileOperationsStateServiceMock.Object);

            await operationsService.MoveAsync(new[] {FileName}, DirectoryName);

            operationMock.Verify(m => m.RunAsync(), Times.Once());
        }

        [Fact]
        public async Task TestFilesCopy()
        {
            var fullPath = Path.Combine(DirectoryName, FileName);
            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync())
                .Verifiable();
            operationsFactoryMock
                .Setup(m => m.CreateCopyOperation(It.IsAny<BinaryFileSystemOperationSettings>()))
                .Callback<BinaryFileSystemOperationSettings>(s =>
                {
                    var (key, value) = s.FilesDictionary.Single();

                    Assert.Equal(FileName,key);
                    Assert.Equal(fullPath, value);
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
            var fileOperationsStateServiceMock = new Mock<IOperationsStateService>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetCommonRootDirectory(It.IsAny<IReadOnlyList<string>>()))
                .Returns(string.Empty);
            pathServiceMock
                .Setup(m => m.Combine(DirectoryName, FileName))
                .Returns(fullPath);
            pathServiceMock
                .Setup(m => m.GetRelativePath(string.Empty, FileName))
                .Returns(FileName);

            IOperationsService operationsService = new OperationsService(
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object,
                pathServiceMock.Object,
                fileOperationsStateServiceMock.Object);

            await operationsService.CopyAsync(new[] {FileName}, DirectoryName);

            operationMock.Verify(m => m.RunAsync(), Times.Once());
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
            var fileOperationsStateServiceMock = new Mock<IOperationsStateService>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.Combine(SelectedDirectoryName, DirectoryName))
                .Returns(fullDirectoryPath);

            IOperationsService operationsService = new OperationsService(
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object,
                pathServiceMock.Object,
                fileOperationsStateServiceMock.Object);

            operationsService.CreateDirectory(SelectedDirectoryName, DirectoryName);

            directoryServiceMock.Verify(m => m.Create(fullDirectoryPath), Times.Once());
        }

        [Fact]
        public void TestFileRenaming()
        {
            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileOpeningServiceMock = new Mock<IResourceOpeningService>();
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.Rename(FileName, NewFileName))
                .Verifiable();
            fileServiceMock
                .Setup(m => m.CheckIfExists(FileName))
                .Returns(true);
            var fileOperationsStateServiceMock = new Mock<IOperationsStateService>();
            var pathServiceMock = new Mock<IPathService>();

            IOperationsService operationsService = new OperationsService(
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object,
                pathServiceMock.Object,
                fileOperationsStateServiceMock.Object);

            operationsService.Rename(FileName, NewFileName);

            fileServiceMock.Verify(m => m.Rename(FileName, NewFileName), Times.Once());
        }

        [Fact]
        public void TestDirectoryRenaming()
        {
            var operationsFactoryMock = new Mock<IOperationsFactory>();
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.Rename(DirectoryName, NewDirectoryName))
                .Verifiable();
            directoryServiceMock
                .Setup(m => m.CheckIfExists(DirectoryName))
                .Returns(true);
            var fileOpeningServiceMock = new Mock<IResourceOpeningService>();
            var fileServiceMock = new Mock<IFileService>();
            var fileOperationsStateServiceMock = new Mock<IOperationsStateService>();
            var pathServiceMock = new Mock<IPathService>();

            IOperationsService operationsService = new OperationsService(
                operationsFactoryMock.Object,
                directoryServiceMock.Object,
                fileOpeningServiceMock.Object,
                fileServiceMock.Object,
                pathServiceMock.Object,
                fileOperationsStateServiceMock.Object);

            operationsService.Rename(DirectoryName, NewDirectoryName);

            directoryServiceMock.Verify(m => m.Rename(DirectoryName, NewDirectoryName), Times.Once());
        }
    }
}