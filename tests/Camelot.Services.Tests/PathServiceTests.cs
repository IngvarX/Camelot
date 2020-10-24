using System.IO;
using Camelot.Services.Environment.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests
{
    public class PathServiceTests
    {
        private const string Directory = "Directory";
        private const string File = "File";

        private static string FullPath => $"{Directory}{Path.DirectorySeparatorChar}{File}";

        private readonly AutoMocker _autoMocker;

        public PathServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("File", "File")]
        [InlineData("File.cs", "File")]
        [InlineData("Camelot.Services.Tests.csproj", "Camelot.Services.Tests")]
        [InlineData(".gitignore", ".gitignore")]
        [InlineData(".travis.yml", ".travis")]
        public void TestNameExtraction(string fileName, string expectedFileName)
        {
            _autoMocker
                .Setup<IEnvironmentPathService, string>(m => m.GetFileNameWithoutExtension(It.IsAny<string>()))
                .Returns<string>(Path.GetFileNameWithoutExtension);
            var pathService = _autoMocker.CreateInstance<PathService>();
            var actualFileName = pathService.GetFileNameWithoutExtension(fileName);

            Assert.Equal(expectedFileName, actualFileName);
        }

        [Theory]
        [InlineData("File", "")]
        [InlineData("File.cs", "cs")]
        [InlineData("Camelot.Services.Tests.csproj", "csproj")]
        [InlineData(".gitignore", "")]
        [InlineData(".travis.yml", "yml")]
        public void TestExtensionExtraction(string fileName, string expectedExtension)
        {
            _autoMocker
                .Setup<IEnvironmentPathService, string>(m => m.GetExtension(It.IsAny<string>()))
                .Returns<string>(Path.GetExtension);
            var pathService = _autoMocker.CreateInstance<PathService>();
            var actualExtension = pathService.GetExtension(fileName);

            Assert.Equal(expectedExtension, actualExtension);
        }

        [Fact]
        public void TestPathCombine()
        {
            _autoMocker
                .Setup<IEnvironmentPathService, string>(m => m.Combine(Directory, File))
                .Returns(FullPath);
            var pathService = _autoMocker.CreateInstance<PathService>();
            var path = pathService.Combine(Directory, File);

            Assert.Equal(FullPath, path);
        }

        [Theory]
        [InlineData("Directory", "Directory")]
        [InlineData("Directory/", "Directory")]
        [InlineData("Directory\\", "Directory")]
        [InlineData("/", "/")]
        public void TestTrimPathSeparators(string directory, string expectedResult)
        {
            var pathService = _autoMocker.CreateInstance<PathService>();
            var path = pathService.TrimPathSeparators(directory);

            Assert.Equal(path, expectedResult);
        }

        [Theory]
        [InlineData(new[] {"/home/test/File1", "/home/test/File2"}, "/home/test")]
        [InlineData(new[] {"/home/test/File1", "/var/test/File2"}, "/")]
        [InlineData(new[] {"/home/test/1", "/home/test/2"}, "/home/test")]
        [InlineData(new string[] {}, null)]
        public void TestGetCommonRootDirectory(string[] files, string expectedDirectory)
        {
            _autoMocker
                .Setup<IEnvironmentPathService, string>(m => m.GetDirectoryName(It.IsAny<string>()))
                .Returns<string>(Path.GetDirectoryName);
            var pathService = _autoMocker.CreateInstance<PathService>();
            var actualDirectory = pathService.GetCommonRootDirectory(files);

            Assert.Equal(expectedDirectory, actualDirectory?.Replace("\\", "/"));
        }

        [Fact]
        public void TestGetRelativePath()
        {
            _autoMocker
                .Setup<IEnvironmentPathService, string>(m => m.GetRelativePath(Directory, File))
                .Returns(FullPath);
            var pathService = _autoMocker.CreateInstance<PathService>();
            var path = pathService.GetRelativePath(Directory, File);

            Assert.Equal(FullPath, path);
        }

        [Fact]
        public void TestGetPathRoot()
        {
            _autoMocker
                .Setup<IEnvironmentPathService, string>(m => m.GetPathRoot(FullPath))
                .Returns(Directory);
            var pathService = _autoMocker.CreateInstance<PathService>();
            var path = pathService.GetPathRoot(FullPath);

            Assert.Equal(Directory, path);
        }
    }
}