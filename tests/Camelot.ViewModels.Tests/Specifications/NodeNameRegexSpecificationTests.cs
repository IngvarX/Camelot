using System.Text.RegularExpressions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Environment.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Specifications;

public class NodeNameRegexSpecificationTests
{
    private const string FileName = "FileName";

    [Theory]
    [InlineData("text", true, RegexOptions.None, true, true)]
    [InlineData("text", true, RegexOptions.None, false, false)]
    [InlineData("text", false, RegexOptions.IgnoreCase, true, true)]
    [InlineData("text", false, RegexOptions.IgnoreCase, false, false)]
    public void TestIsSatisfiedBy(string regex, bool isCaseSensitive, RegexOptions options, bool expected,
        bool isRecursive)
    {
        var regexServiceMock = new Mock<IRegexService>();
        regexServiceMock
            .Setup(m => m.CheckIfMatches(FileName, regex, options))
            .Returns(expected);
        var specification = new NodeNameRegexSpecification(regexServiceMock.Object, regex, isCaseSensitive, isRecursive);

        Assert.Equal(isRecursive, specification.IsRecursive);
        var nodeModel = new FileModel
        {
            Name = FileName
        };
        var actual = specification.IsSatisfiedBy(nodeModel);

        Assert.Equal(expected, actual);
    }
}