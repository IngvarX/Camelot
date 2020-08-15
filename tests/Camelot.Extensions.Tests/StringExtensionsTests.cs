using Xunit;

namespace Camelot.Extensions.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData("t", "T")]
        [InlineData("E", "E")]
        [InlineData("st", "St")]
        [InlineData("Tests", "Tests")]
        public void TestToTitleCase(string source, string expected)
        {
            var actual = source.ToTitleCase();

            Assert.Equal(expected, actual);
        }
    }
}