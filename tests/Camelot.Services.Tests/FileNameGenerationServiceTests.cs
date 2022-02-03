using System.Linq;
using Camelot.Services.Abstractions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests;

public class FileNameGenerationServiceTests
{
    private readonly AutoMocker _autoMocker;

    public FileNameGenerationServiceTests()
    {
        _autoMocker = new AutoMocker();

        _autoMocker
            .Setup<IPathService, string>(m => m.Combine(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string a, string b) => $"{a}/{b}");
        _autoMocker
            .Setup<IPathService, string>(m => m.GetFileName(It.IsAny<string>()))
            .Returns((string fileName) => fileName.Split("/")[^1]);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetFileNameWithoutExtension(It.IsAny<string>()))
            .Returns((string fileName) => fileName.Split("/")[^1].Split(".")[0]);
        _autoMocker
            .Setup<IPathService, string>(m => m.GetParentDirectory(It.IsAny<string>()))
            .Returns("test");
        _autoMocker
            .Setup<IPathService, string>(m => m.GetExtension(It.IsAny<string>()))
            .Returns((string fileName) => fileName.Contains(".") ? fileName.Split(".")[1] : string.Empty);
    }

    [Theory]
    [InlineData("file.txt", "file.txt", new string[] { })]
    [InlineData("file.txt", "file (2).txt", new[] {"/file.txt", "/file (1).txt"})]
    [InlineData("file.txt", "file (1).txt", new[] {"/file.txt", "/file (1).pdf"})]
    [InlineData("file.txt", "file (1).txt", new[] {"/file.txt", "/file (2).txt"})]
    [InlineData("file.txt", "file (3).txt", new[] {"/file.txt", "/file (1).txt", "/file (2).txt"})]
    [InlineData("directory", "directory", new string[] { })]
    [InlineData("directory", "directory (2)", new[] {"/directory", "/directory (1)"})]
    [InlineData("directory", "directory (1)", new[] {"/directory", "/directory (2)"})]
    [InlineData("directory", "directory (3)", new[] {"/directory", "/directory (1)", "/directory (2)"})]
    public void TestNames(string initialName, string outputName, string[] existingFiles)
    {
        _autoMocker
            .Setup<INodeService, bool>(m => m.CheckIfExists(It.IsAny<string>()))
            .Returns((string file) => existingFiles.Contains(file));

        var fileNameGenerationService = _autoMocker.CreateInstance<FileNameGenerationService>();
        var newName = fileNameGenerationService.GenerateName(initialName, string.Empty);

        Assert.Equal(outputName, newName);
    }

    [Theory]
    [InlineData("file.txt", "test/file.txt", new string[] { })]
    [InlineData("file.txt", "test/file (2).txt", new[] {"test/file.txt", "test/file (1).txt"})]
    [InlineData("file.txt", "test/file (1).txt", new[] {"test/file.txt", "test/file (1).pdf"})]
    [InlineData("file.txt", "test/file (1).txt", new[] {"test/file.txt", "test/file (2).txt"})]
    [InlineData("file.txt", "test/file (3).txt", new[] {"test/file.txt", "test/file (1).txt", "test/file (2).txt"})]
    [InlineData("directory", "test/directory", new string[] { })]
    [InlineData("directory", "test/directory (2)", new[] {"test/directory", "test/directory (1)"})]
    [InlineData("directory", "test/directory (1)", new[] {"test/directory", "test/directory (2)"})]
    [InlineData("directory", "test/directory (3)", new[] {"test/directory", "test/directory (1)", "test/directory (2)"})]
    public void TestFullNames(string initialName, string outputName, string[] existingFiles)
    {
        _autoMocker
            .Setup<INodeService, bool>(m => m.CheckIfExists(It.IsAny<string>()))
            .Returns((string file) => existingFiles.Contains(file));

        var fileNameGenerationService = _autoMocker.CreateInstance<FileNameGenerationService>();
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
    [InlineData("directory", "test/directory", new string[] { })]
    [InlineData("directory", "test/directory (2)", new[] {"test/directory", "test/directory (1)"})]
    [InlineData("directory", "test/directory (1)", new[] {"test/directory", "test/directory (2)"})]
    [InlineData("directory", "test/directory (3)", new[] {"test/directory", "test/directory (1)", "test/directory (2)"})]
    public void TestFullNamesWithoutExtension(string initialName, string outputName, string[] existingFiles)
    {
        _autoMocker
            .Setup<INodeService, bool>(m => m.CheckIfExists(It.IsAny<string>()))
            .Returns((string file) => existingFiles.Contains(file));

        var fileNameGenerationService = _autoMocker.CreateInstance<FileNameGenerationService>();
        var newName = fileNameGenerationService.GenerateFullNameWithoutExtension(initialName);

        Assert.Equal(outputName, newName);
    }
}