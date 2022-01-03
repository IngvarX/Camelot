using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests;

public class NodeServiceTests
{
    private const string Node = "Node";

    private readonly AutoMocker _autoMocker;

    public NodeServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, true)]
    public void TestCheckIfExists(bool fileExists, bool dirExists, bool expected)
    {
        _autoMocker
            .Setup<IFileService, bool>(m => m.CheckIfExists(Node))
            .Returns(fileExists);
        _autoMocker
            .Setup<IDirectoryService, bool>(m => m.CheckIfExists(Node))
            .Returns(dirExists);

        var service = _autoMocker.CreateInstance<NodeService>();
        var actual = service.CheckIfExists(Node);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TestGetNode(bool fileExists)
    {
        var fileNode = new FileModel();
        var directoryNode = new DirectoryModel();

        _autoMocker
            .Setup<IFileService, bool>(m => m.CheckIfExists(Node))
            .Returns(fileExists);
        _autoMocker
            .Setup<IFileService, FileModel>(m => m.GetFile(Node))
            .Returns(fileNode);
        _autoMocker
            .Setup<IDirectoryService, DirectoryModel>(m => m.GetDirectory(Node))
            .Returns(directoryNode);

        var service = _autoMocker.CreateInstance<NodeService>();
        var node = service.GetNode(Node);

        Assert.Equal(fileExists ? (NodeModelBase) fileNode : directoryNode, node);
    }
}