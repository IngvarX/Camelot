using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;
using Xunit;

namespace Camelot.ViewModels.Tests.Specifications;

public class NodeNameTextSpecificationTests
{
    [Theory]
    [InlineData("text", true, "text.txt", true, true)]
    [InlineData("131311", true, "text.txt", false, false)]
    [InlineData("tExt", true, "text.txt", false, true)]
    [InlineData("tExt", false, "text.txt", true, false)]
    [InlineData("tExt", false, "teXt.txt", true, true)]
    [InlineData("tEx131t", false, "teXt.txt", false, false)]
    public void TestIsSatisfiedBy(string text, bool isCaseSensitive, string fileName, bool expected,
        bool isRecursive)
    {
        var specification = new NodeNameTextSpecification(text, isCaseSensitive, isRecursive);

        Assert.Equal(isRecursive, specification.IsRecursive);
        var nodeModel = new FileModel
        {
            Name = fileName
        };
        var actual = specification.IsSatisfiedBy(nodeModel);

        Assert.Equal(expected, actual);
    }
}