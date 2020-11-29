using Camelot.Services.Abstractions.Models;
using Camelot.Services.Linux.Specifications;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Linux.Tests
{
    public class DesktopFileSpecificationTests
    {
        private readonly AutoMocker _autoMocker;

        public DesktopFileSpecificationTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("desktop", true)]
        [InlineData("desktop ", false)]
        [InlineData("Desktop", false)]
        [InlineData("DesKtop", false)]
        [InlineData("pdf", false)]
        public void Test(string extension, bool expected)
        {
            var fileModel = new FileModel
            {
                Extension = extension
            };
            var specification = _autoMocker.CreateInstance<DesktopFileSpecification>();
            var actual = specification.IsSatisfiedBy(fileModel);

            Assert.Equal(expected, actual);
        }
    }
}