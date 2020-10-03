using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;
using Xunit;

namespace Camelot.ViewModels.Tests.Specifications
{
    public class NodeNameTextSpecificationTests
    {
        [Theory]
        [InlineData("text", true, "text.txt", true)]
        [InlineData("131311", true, "text.txt", false)]
        [InlineData("tExt", true, "text.txt", false)]
        [InlineData("tExt", false, "text.txt", true)]
        [InlineData("tExt", false, "teXt.txt", true)]
        [InlineData("tEx131t", false, "teXt.txt", false)]
        public void TestIsSatisfiedBy(string text, bool isCaseSensitive, string fileName, bool expected)
        {
            var specification = new NodeNameTextSpecification(text, isCaseSensitive);
            var nodeModel = new FileModel
            {
                Name = fileName
            };
            var actual = specification.IsSatisfiedBy(nodeModel);

            Assert.Equal(expected, actual);
        }
    }
}