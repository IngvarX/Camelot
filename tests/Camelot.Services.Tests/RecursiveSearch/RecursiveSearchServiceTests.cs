using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.RecursiveSearch;
using Camelot.Services.Abstractions.Specifications;
using Camelot.Services.RecursiveSearch;
using Camelot.Tests.Shared.Extensions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests.RecursiveSearch;

public class RecursiveSearchServiceTests
{
    private const string Directory = "Dir";

    private readonly AutoMocker _autoMocker;

    public RecursiveSearchServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData("File", true, true, 1)]
    [InlineData("File", true, false, 0)]
    [InlineData("Dir", false, true, 1)]
    [InlineData("Dir", false, false, 0)]
    public async Task TestSearch(string node, bool isFile, bool isSatisfiedBy, int expectedCallsCount)
    {
        var searchResult = new Mock<IRecursiveSearchResult>().Object;
        var publisherMock = new Mock<INodeFoundEventPublisher>();
        publisherMock
            .Setup(m => m.RaiseNodeFoundEvent(node))
            .Verifiable();

        Func<INodeFoundEventPublisher, Task> factory = null;

        _autoMocker
            .Setup<IFileService, bool>(m => m.CheckIfExists(node))
            .Returns(isFile);
        _autoMocker
            .Setup<IFileService, FileModel>(m => m.GetFile(node))
            .Returns(new FileModel
            {
                FullPath = node
            });
        _autoMocker
            .Setup<IDirectoryService, DirectoryModel>(m => m.GetDirectory(node))
            .Returns(new DirectoryModel()
            {
                FullPath = node
            });
        _autoMocker
            .Setup<IDirectoryService, IEnumerable<string>>(m => m.GetNodesRecursively(Directory))
            .Returns(new[] {node});
        _autoMocker
            .Setup<IRecursiveSearchResultFactory, IRecursiveSearchResult>(m =>
                m.Create(It.IsAny<Func<INodeFoundEventPublisher, Task>>()))
            .Callback<Func<INodeFoundEventPublisher, Task>>(f => factory = f)
            .Returns(searchResult);
        var specification = new Mock<ISpecification<NodeModelBase>>();
        specification
            .Setup(m => m.IsSatisfiedBy(It.Is<NodeModelBase>(n => n.FullPath == node)))
            .Returns(isSatisfiedBy);

        var service = _autoMocker.CreateInstance<RecursiveSearchService>();

        var result = service.Search(Directory, specification.Object, default);

        Assert.Equal(searchResult, result);

        Assert.NotNull(factory);
        await factory(publisherMock.Object);

        publisherMock
            .Verify(m => m.RaiseNodeFoundEvent(node),
                Times.Exactly(expectedCallsCount));
    }

    [Fact]
    public async Task TestSearchThrows()
    {
        _autoMocker.MockLogError();
        var publisherMock = new Mock<INodeFoundEventPublisher>();
        publisherMock
            .Setup(m => m.RaiseNodeFoundEvent(Directory))
            .Verifiable();
        _autoMocker
            .Setup<IDirectoryService, IEnumerable<string>>(m => m.GetNodesRecursively(Directory))
            .Returns(new[] {Directory});
        _autoMocker
            .Setup<IDirectoryService>(m => m.GetDirectory(Directory))
            .Throws<InvalidOperationException>();
        var specification = new Mock<ISpecification<NodeModelBase>>();
        Func<INodeFoundEventPublisher, Task> factory = null;
        _autoMocker
            .Setup<IRecursiveSearchResultFactory, IRecursiveSearchResult>(m =>
                m.Create(It.IsAny<Func<INodeFoundEventPublisher, Task>>()))
            .Callback<Func<INodeFoundEventPublisher, Task>>(f => factory = f);

        var service = _autoMocker.CreateInstance<RecursiveSearchService>();

        service.Search(Directory, specification.Object, default);

        Assert.NotNull(factory);

        await factory(publisherMock.Object);

        _autoMocker.VerifyLogError(Times.Once());
        publisherMock
            .Verify(m => m.RaiseNodeFoundEvent(Directory), Times.Never);
    }
}