using System.IO;
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
                .Returns((string a, string b) => Path.Combine(a, b));
            pathServiceMock
                .Setup(m => m.GetFileName(It.IsAny<string>()))
                .Returns((string fileName) => fileName.Split(".")[0]);
            pathServiceMock
                .Setup(m => m.GetExtension(It.IsAny<string>()))
                .Returns((string fileName) => fileName.Contains(".") ? fileName.Split(".")[1] : string.Empty);
            _pathService = pathServiceMock.Object;
        }

        [Theory]
        [InlineData("file.txt", "file.txt", new string[] { })]
        [InlineData("file.txt", "file (2).txt", new[] {"file.txt", "file (1).txt"})]
        [InlineData("file.txt", "file (1).txt", new[] {"file.txt", "file (2).txt"})]
        [InlineData("file.txt", "file (3).txt", new[] {"file.txt", "file (1).txt", "file (2).txt"})]
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
        [InlineData("directory", "directory", new string[] { })]
        [InlineData("directory", "directory (2)", new[] {"directory", "directory (1)"})]
        [InlineData("directory", "directory (1)", new[] {"directory", "directory (2)"})]
        [InlineData("directory", "directory (3)", new[] {"directory", "directory (1)", "directory (2)"})]
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
    }
}