using System.Linq;
using Camelot.Services.Abstractions;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class FileNameGenerationServiceTests
    {
        private readonly IPathService _pathService;

        public FileNameGenerationServiceTests()
        {
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.Combine(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string a, string b) => $"{a}/{b}");
            pathServiceMock
                .Setup(m => m.GetFileName(It.IsAny<string>()))
                .Returns((string fileName) => fileName.Split("/")[^1]);
            pathServiceMock
                .Setup(m => m.GetFileNameWithoutExtension(It.IsAny<string>()))
                .Returns((string fileName) => fileName.Split("/")[^1].Split(".")[0]);
            pathServiceMock
                .Setup(m => m.GetParentDirectory(It.IsAny<string>()))
                .Returns("test");
            pathServiceMock
                .Setup(m => m.GetExtension(It.IsAny<string>()))
                .Returns((string fileName) => fileName.Contains(".") ? fileName.Split(".")[1] : string.Empty);
            _pathService = pathServiceMock.Object;
        }

        [Theory]
        [InlineData("file.txt", "file.txt", new string[] { })]
        [InlineData("file.txt", "file (2).txt", new[] {"/file.txt", "/file (1).txt"})]
        [InlineData("file.txt", "file (1).txt", new[] {"/file.txt", "/file (1).pdf"})]
        [InlineData("file.txt", "file (1).txt", new[] {"/file.txt", "/file (2).txt"})]
        [InlineData("file.txt", "file (3).txt", new[] {"/file.txt", "/file (1).txt", "/file (2).txt"})]
        public void TestFileNames(string initialName, string outputName, string[] existingFiles)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns(false);

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns((string file) => existingFiles.Contains(file));

            var fileNameGenerationService = new FileNameGenerationService(
                fileServiceMock.Object, directoryServiceMock.Object, _pathService);
            var newName = fileNameGenerationService.GenerateName(initialName, string.Empty);

            Assert.Equal(outputName, newName);
        }

        [Theory]
        [InlineData("file.txt", "test/file.txt", new string[] { })]
        [InlineData("file.txt", "test/file (2).txt", new[] {"test/file.txt", "test/file (1).txt"})]
        [InlineData("file.txt", "test/file (1).txt", new[] {"test/file.txt", "test/file (1).pdf"})]
        [InlineData("file.txt", "test/file (1).txt", new[] {"test/file.txt", "test/file (2).txt"})]
        [InlineData("file.txt", "test/file (3).txt", new[] {"test/file.txt", "test/file (1).txt", "test/file (2).txt"})]
        public void TestFullFileNames(string initialName, string outputName, string[] existingFiles)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns(false);

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns((string file) => existingFiles.Contains(file));

            var fileNameGenerationService = new FileNameGenerationService(
                fileServiceMock.Object, directoryServiceMock.Object, _pathService);
            var newName = fileNameGenerationService.GenerateFullName(initialName);

            Assert.Equal(outputName, newName);
        }

        [Theory]
        [InlineData("file.txt", "test/file", new string[] { })]
        [InlineData("file.txt", "test/file (2)", new[] {"test/file", "test/file (1)"})]
        [InlineData("file.txt", "test/file", new[] {"test/file.txt", "test/file (1).pdf"})]
        [InlineData("file.txt", "test/file (1)", new[] {"test/file", "test/file (1).pdf"})]
        [InlineData("file.txt", "test/file (1)", new[] {"test/file", "test/file (2)"})]
        [InlineData("file.txt", "test/file (3)", new[] {"test/file", "test/file (1)", "test/file (2)"})]
        public void TestFullFileNamesWithoutExtension(string initialName, string outputName, string[] existingFiles)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns(false);

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns((string file) => existingFiles.Contains(file));

            var fileNameGenerationService = new FileNameGenerationService(
                fileServiceMock.Object, directoryServiceMock.Object, _pathService);
            var newName = fileNameGenerationService.GenerateFullNameWithoutExtension(initialName);

            Assert.Equal(outputName, newName);
        }

        [Theory]
        [InlineData("directory", "directory", new string[] { })]
        [InlineData("directory", "directory (2)", new[] {"/directory", "/directory (1)"})]
        [InlineData("directory", "directory (1)", new[] {"/directory", "/directory (2)"})]
        [InlineData("directory", "directory (3)", new[] {"/directory", "/directory (1)", "/directory (2)"})]
        public void TestDirectoryNames(string initialName, string outputName, string[] existingDirs)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns((string dirName) => existingDirs.Contains(dirName));

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns(false);

            var fileNameGenerationService = new FileNameGenerationService(
                fileServiceMock.Object, directoryServiceMock.Object, _pathService);
            var newName = fileNameGenerationService.GenerateName(initialName, string.Empty);

            Assert.Equal(outputName, newName);
        }

        [Theory]
        [InlineData("directory", "test/directory", new string[] { })]
        [InlineData("directory", "test/directory (2)", new[] {"test/directory", "test/directory (1)"})]
        [InlineData("directory", "test/directory (1)", new[] {"test/directory", "test/directory (2)"})]
        [InlineData("directory", "test/directory (3)", new[] {"test/directory", "test/directory (1)", "test/directory (2)"})]
        public void TestFullDirNames(string initialName, string outputName, string[] existingDirs)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns((string dirName) => existingDirs.Contains(dirName));

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns(false);

            var fileNameGenerationService = new FileNameGenerationService(
                fileServiceMock.Object, directoryServiceMock.Object, _pathService);
            var newName = fileNameGenerationService.GenerateFullName(initialName);

            Assert.Equal(outputName, newName);
        }

        [Theory]
        [InlineData("directory", "test/directory", new string[] { })]
        [InlineData("directory", "test/directory (2)", new[] {"test/directory", "test/directory (1)"})]
        [InlineData("directory", "test/directory (1)", new[] {"test/directory", "test/directory (2)"})]
        [InlineData("directory", "test/directory (3)", new[] {"test/directory", "test/directory (1)", "test/directory (2)"})]
        public void TestFullDirNamesWithoutExtension(string initialName, string outputName, string[] existingDirs)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns((string dirName) => existingDirs.Contains(dirName));

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfExists(It.IsAny<string>()))
                .Returns(false);

            var fileNameGenerationService = new FileNameGenerationService(
                fileServiceMock.Object, directoryServiceMock.Object, _pathService);
            var newName = fileNameGenerationService.GenerateFullNameWithoutExtension(initialName);

            Assert.Equal(outputName, newName);
        }
    }
}