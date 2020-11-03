using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Operations.Tests
{
    public class OperationsServiceTests
    {
        private const string FileName = "FileName";
        private const string NewFileName = "NewFileName";
        private const string SelectedDirectoryName = "SelectedDirectoryName";
        private const string DirectoryName = "DirectoryName";
        private const string NewDirectoryName = "NewDirectoryName";

        private readonly AutoMocker _autoMocker;

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

        public OperationsServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestEditSelectedFiles()
        {
            _autoMocker
                .Setup<IResourceOpeningService>(m => m.Open(FileName))
                .Verifiable();

            var operationsService = _autoMocker.CreateInstance<OperationsService>();

            operationsService.OpenFiles(new[] {FileName});

            _autoMocker
                .Verify<IResourceOpeningService>(m => m.Open(FileName), Times.Once);
        }

        [Fact]
        public async Task TestFilesRemoving()
        {
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync())
                .Verifiable();
            _autoMocker
                .Setup<IOperationsFactory, IOperation>(m => m.CreateDeleteOperation(It.IsAny<UnaryFileSystemOperationSettings>()))
                .Callback<UnaryFileSystemOperationSettings>(s =>
                {
                    Assert.Equal(FileName, s.TopLevelFiles.Single());
                })
                .Returns(operationMock.Object);

            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(FileName))
                .Returns(true)
                .Verifiable();
            _autoMocker
                .Setup<IOperationsStateService>(m => m.AddOperation(operationMock.Object))
                .Verifiable();

            var operationsService = _autoMocker.CreateInstance<OperationsService>();

            await operationsService.RemoveAsync(new[] {FileName});

            operationMock.Verify(m => m.RunAsync(), Times.Once);
            _autoMocker
                .Verify<IFileService, bool>(m => m.CheckIfExists(FileName), Times.Once);
            _autoMocker
                .Verify<IOperationsStateService>(m => m.AddOperation(operationMock.Object), Times.Once);
        }

        [Theory]
        [InlineData(ArchiveType.Zip)]
        [InlineData(ArchiveType.Gz)]
        [InlineData(ArchiveType.Tar)]
        [InlineData(ArchiveType.SevenZip)]
        [InlineData(ArchiveType.TarGz)]
        [InlineData(ArchiveType.TarBz2)]
        [InlineData(ArchiveType.Bz2)]
        [InlineData(ArchiveType.TarXz)]
        [InlineData(ArchiveType.Xz)]
        [InlineData(ArchiveType.TarLz)]
        [InlineData(ArchiveType.Lz)]
        public async Task TestFilesPack(ArchiveType archiveType)
        {
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync())
                .Verifiable();
            _autoMocker
                .Setup<IOperationsFactory, IOperation>(m => m.CreatePackOperation(
                    It.Is<PackOperationSettings>(s => s.ArchiveType == archiveType && s.InputTopLevelFiles.Single() == FileName)))
                .Returns(operationMock.Object);

            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(FileName))
                .Returns(true)
                .Verifiable();
            _autoMocker
                .Setup<IOperationsStateService>(m => m.AddOperation(operationMock.Object))
                .Verifiable();

            var operationsService = _autoMocker.CreateInstance<OperationsService>();

            await operationsService.PackAsync(new[] {FileName}, NewFileName, archiveType);

            operationMock.Verify(m => m.RunAsync(), Times.Once);
            _autoMocker
                .Verify<IFileService, bool>(m => m.CheckIfExists(FileName), Times.Once);
            _autoMocker
                .Verify<IOperationsStateService>(m => m.AddOperation(operationMock.Object), Times.Once);
        }

        [Theory]
        [InlineData(ArchiveType.Zip)]
        [InlineData(ArchiveType.Gz)]
        [InlineData(ArchiveType.Tar)]
        [InlineData(ArchiveType.SevenZip)]
        [InlineData(ArchiveType.TarGz)]
        [InlineData(ArchiveType.TarBz2)]
        [InlineData(ArchiveType.Bz2)]
        [InlineData(ArchiveType.TarXz)]
        [InlineData(ArchiveType.Xz)]
        [InlineData(ArchiveType.TarLz)]
        [InlineData(ArchiveType.Lz)]
        public async Task TestFilesExtract(ArchiveType archiveType)
        {
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync())
                .Verifiable();
            _autoMocker
                .Setup<IOperationsFactory, IOperation>(m => m.CreateExtractOperation(
                    It.Is<ExtractArchiveOperationSettings>(s => s.ArchiveType == archiveType && s.InputTopLevelFile == FileName && s.TargetDirectory == DirectoryName)))
                .Returns(operationMock.Object);

            _autoMocker
                .Setup<IOperationsStateService>(m => m.AddOperation(operationMock.Object))
                .Verifiable();

            var operationsService = _autoMocker.CreateInstance<OperationsService>();

            await operationsService.ExtractAsync(FileName, DirectoryName, archiveType);

            operationMock.Verify(m => m.RunAsync(), Times.Once);
            _autoMocker
                .Verify<IOperationsStateService>(m => m.AddOperation(operationMock.Object), Times.Once);
        }

        [Fact]
        public async Task TestFilesMove()
        {
            var fullPath = Path.Combine(DirectoryName, FileName);
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync())
                .Verifiable();
            _autoMocker
                .Setup<IOperationsFactory, IOperation>(m => m.CreateMoveOperation(It.IsAny<BinaryFileSystemOperationSettings>()))
                .Callback<BinaryFileSystemOperationSettings>(s =>
                {
                    var (key, value) = s.FilesDictionary.Single();

                    Assert.Equal(FileName, key);
                    Assert.Equal(fullPath, value);
                })
                .Returns(operationMock.Object);

            _autoMocker
                .Setup<IDirectoryService, string>(m => m.SelectedDirectory)
                .Returns(CurrentDirectory);
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(FileName))
                .Returns(true);
            _autoMocker
                .Setup<IOperationsStateService>(m => m.AddOperation(operationMock.Object))
                .Verifiable();
            _autoMocker
                .Setup<IPathService, string>(m => m.GetCommonRootDirectory(It.IsAny<IReadOnlyList<string>>()))
                .Returns(string.Empty);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(DirectoryName, FileName))
                .Returns(fullPath);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetRelativePath(string.Empty, FileName))
                .Returns(FileName);

            var operationsService = _autoMocker.CreateInstance<OperationsService>();

            await operationsService.MoveAsync(new[] {FileName}, DirectoryName);

            operationMock.Verify(m => m.RunAsync(), Times.Once);
            _autoMocker
                .Verify<IOperationsStateService>(m => m.AddOperation(operationMock.Object), Times.Once);
        }

        [Fact]
        public async Task TestFilesCopy()
        {
            var fullPath = Path.Combine(DirectoryName, FileName);
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync())
                .Verifiable();
            _autoMocker
                .Setup<IOperationsFactory, IOperation>(m => m.CreateCopyOperation(It.IsAny<BinaryFileSystemOperationSettings>()))
                .Callback<BinaryFileSystemOperationSettings>(s =>
                {
                    var (key, value) = s.FilesDictionary.Single();

                    Assert.Equal(FileName,key);
                    Assert.Equal(fullPath, value);
                })
                .Returns(operationMock.Object);

            _autoMocker
                .Setup<IDirectoryService, string>(m => m.SelectedDirectory)
                .Returns(CurrentDirectory);
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(FileName))
                .Returns(true);
            _autoMocker
                .Setup<IOperationsStateService>(m => m.AddOperation(operationMock.Object))
                .Verifiable();
            _autoMocker
                .Setup<IPathService, string>(m => m.GetCommonRootDirectory(It.IsAny<IReadOnlyList<string>>()))
                .Returns(string.Empty);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(DirectoryName, FileName))
                .Returns(fullPath);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetRelativePath(string.Empty, FileName))
                .Returns(FileName);

            var operationsService = _autoMocker.CreateInstance<OperationsService>();

            await operationsService.CopyAsync(new[] {FileName}, DirectoryName);

            operationMock.Verify(m => m.RunAsync(), Times.Once);
            _autoMocker
                .Verify<IOperationsStateService>(m => m.AddOperation(operationMock.Object), Times.Once);
        }

        [Fact]
        public async Task TestMoveByDictionary()
        {
            var fullPath = Path.Combine(DirectoryName, FileName);
            var newFullPath = Path.Combine(NewDirectoryName, NewFileName);
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync())
                .Verifiable();
            _autoMocker
                .Setup<IOperationsFactory, IOperation>(m => m.CreateMoveOperation(It.Is<BinaryFileSystemOperationSettings>(s =>
                    s.FilesDictionary.ContainsKey(fullPath) && s.FilesDictionary[fullPath] == newFullPath)))
                .Returns(operationMock.Object);

            _autoMocker
                .Setup<IDirectoryService, string>(m => m.SelectedDirectory)
                .Returns(CurrentDirectory);
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(fullPath))
                .Returns(true);
            _autoMocker
                .Setup<IOperationsStateService>(m => m.AddOperation(operationMock.Object))
                .Verifiable();
            _autoMocker
                .Setup<IPathService, string>(m => m.GetCommonRootDirectory(It.IsAny<IReadOnlyList<string>>()))
                .Returns(string.Empty);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(DirectoryName, FileName))
                .Returns(fullPath);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetRelativePath(string.Empty, FileName))
                .Returns(FileName);

            var operationsService = _autoMocker.CreateInstance<OperationsService>();

            await operationsService.MoveAsync(new Dictionary<string, string> {[fullPath] = newFullPath});

            operationMock.Verify(m => m.RunAsync(), Times.Once);
            _autoMocker
                .Verify<IOperationsStateService>(m => m.AddOperation(operationMock.Object), Times.Once);
        }

        [Fact]
        public async Task TestMoveEmptyDirectoryByDictionary()
        {
            var operationMock = new Mock<IOperation>();
            operationMock
                .Setup(m => m.RunAsync())
                .Verifiable();
            _autoMocker
                .Setup<IOperationsFactory, IOperation>(m => m.CreateMoveOperation(It.Is<BinaryFileSystemOperationSettings>(s =>
                    s.EmptyDirectories.Single() == NewDirectoryName)))
                .Returns(operationMock.Object);

            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(DirectoryName))
                .Returns(true);
            _autoMocker
                .Setup<IDirectoryService, IReadOnlyList<string>>(m => m.GetEmptyDirectoriesRecursively(DirectoryName))
                .Returns(new[] {DirectoryName});
            _autoMocker
                .Setup<IOperationsStateService>(m => m.AddOperation(operationMock.Object))
                .Verifiable();
            _autoMocker
                .Setup<IPathService, string>(m => m.GetCommonRootDirectory(It.IsAny<IReadOnlyList<string>>()))
                .Returns(string.Empty);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetRelativePath(string.Empty, DirectoryName))
                .Returns(DirectoryName);
            _autoMocker
                .Setup<IPathService, string>(m => m.GetRelativePath(DirectoryName, DirectoryName))
                .Returns(string.Empty);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(NewDirectoryName, string.Empty))
                .Returns(NewDirectoryName);
            _autoMocker
                .Setup<IDirectoryService, IReadOnlyList<string>>(m => m.GetFilesRecursively(It.IsAny<string>()))
                .Returns(new List<string>());

            var operationsService = _autoMocker.CreateInstance<OperationsService>();

            await operationsService.MoveAsync(new Dictionary<string, string> {[DirectoryName] = NewDirectoryName});

            operationMock.Verify(m => m.RunAsync(), Times.Once);
            _autoMocker
                .Verify<IOperationsStateService>(m => m.AddOperation(operationMock.Object), Times.Once);
        }

        [Fact]
        public void TestDirectoryCreation()
        {
            var fullDirectoryPath = Path.Combine(SelectedDirectoryName, DirectoryName);
            _autoMocker
                .Setup<IDirectoryService>(m => m.Create(fullDirectoryPath))
                .Verifiable();
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(SelectedDirectoryName, DirectoryName))
                .Returns(fullDirectoryPath);

            var operationsService = _autoMocker.CreateInstance<OperationsService>();

            operationsService.CreateDirectory(SelectedDirectoryName, DirectoryName);

            _autoMocker
                .Verify<IDirectoryService, bool>(m => m.Create(fullDirectoryPath), Times.Once);
        }

        [Fact]
        public void TestFileCreation()
        {
            var fullDirectoryPath = Path.Combine(SelectedDirectoryName, DirectoryName);
            _autoMocker
                .Setup<IFileService>(m => m.CreateFile(fullDirectoryPath))
                .Verifiable();
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(SelectedDirectoryName, FileName))
                .Returns(fullDirectoryPath);

            var operationsService = _autoMocker.CreateInstance<OperationsService>();

            operationsService.CreateFile(SelectedDirectoryName, FileName);

            _autoMocker
                .Verify<IFileService>(m => m.CreateFile(fullDirectoryPath), Times.Once);
        }

        [Fact]
        public void TestFileRenaming()
        {
            _autoMocker
                .Setup<IFileService>(m => m.Rename(FileName, NewFileName))
                .Verifiable();
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(FileName))
                .Returns(true);

            var operationsService = _autoMocker.CreateInstance<OperationsService>();

            operationsService.Rename(FileName, NewFileName);

            _autoMocker
                .Verify<IFileService, bool>(m => m.Rename(FileName, NewFileName), Times.Once);
        }

        [Fact]
        public void TestDirectoryRenaming()
        {
            _autoMocker
                .Setup<IDirectoryService>(m => m.Rename(DirectoryName, NewDirectoryName))
                .Verifiable();
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(DirectoryName))
                .Returns(true);

            var operationsService = _autoMocker.CreateInstance<OperationsService>();

            operationsService.Rename(DirectoryName, NewDirectoryName);

            _autoMocker
                .Verify<IDirectoryService, bool>(m => m.Rename(DirectoryName, NewDirectoryName), Times.Once);
        }
    }
}