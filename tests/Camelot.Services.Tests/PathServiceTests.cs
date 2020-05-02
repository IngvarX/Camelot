using System.IO;
using Camelot.Services.Abstractions;
using Xunit;

namespace Camelot.Services.Tests
{
    public class PathServiceTests
    {
        private const string Directory = "Directory";
        private const string File = "File";

        private readonly IPathService _pathService;

        private static string FullPath => $"{Directory}{Path.DirectorySeparatorChar}{File}";

        public PathServiceTests()
        {
            _pathService = new PathService();
        }

        [Theory]
        [InlineData("File", "File")]
        [InlineData("File.cs", "File")]
        [InlineData("Camelot.Services.Tests.csproj", "Camelot.Services.Tests")]
        [InlineData(".gitignore", ".gitignore")]
        [InlineData(".travis.yml", ".travis")]
        public void TestNameExtraction(string fileName, string expectedFileName)
        {
            var actualFileName = _pathService.GetFileNameWithoutExtension(fileName);

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
            var actualExtension = _pathService.GetExtension(fileName);

            Assert.Equal(expectedExtension, actualExtension);
        }

        [Fact]
        public void TestPathCombine()
        {
            var path = _pathService.Combine(Directory, File);

            Assert.Equal(FullPath, path);
        }
        
        [Theory]
        [InlineData("Directory", "Directory")]
        [InlineData("Directory/", "Directory")]
        [InlineData("Directory\\", "Directory")]
        [InlineData("/", "/")]
        public void TestTrimPathSeparators(string directory, string expectedResult)
        {
            var path = _pathService.TrimPathSeparators(directory);

            Assert.Equal(path, expectedResult);
        }
    }
}