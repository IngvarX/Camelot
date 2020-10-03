using System.Text.RegularExpressions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Environment.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Specifications
{
    public class NodeNameRegexSpecificationTests
    {
        private const string FileName = "FileName";

        [Theory]
        [InlineData("text", true, RegexOptions.None, true)]
        [InlineData("text", true, RegexOptions.None, false)]
        [InlineData("text", false, RegexOptions.IgnoreCase, true)]
        [InlineData("text", false, RegexOptions.IgnoreCase, false)]
        public void TestIsSatisfiedBy(string regex, bool isCaseSensitive, RegexOptions options, bool expected)
        {
            var regexServiceMock = new Mock<IRegexService>();
            regexServiceMock
                .Setup(m => m.CheckIfMatches(FileName, regex, options))
                .Returns(expected);
            var specification = new NodeNameRegexSpecification(regexServiceMock.Object, regex, isCaseSensitive);
            var nodeModel = new FileModel
            {
                Name = FileName
            };
            var actual = specification.IsSatisfiedBy(nodeModel);

            Assert.Equal(expected, actual);
        }
    }
}