using System;
using Camelot.Services.Abstractions;
using Xunit;

namespace Camelot.Services.Tests
{
    public class FileSizeFormatterTests
    {
        private readonly IFileSizeFormatter _fileSizeFormatter;

        public FileSizeFormatterTests()
        {
            _fileSizeFormatter = new FileSizeFormatter();
        }

        [Theory]
        [InlineData(0, "0 B")]
        [InlineData(1, "1 B")]
        [InlineData(1000, "1000 B")]
        [InlineData(1025, "1 KB")]
        [InlineData(1127, "1.1 KB")]
        [InlineData(1024L * 1024L, "1 MB")]
        [InlineData(512 * 1024L * 1024L, "512 MB")]
        [InlineData(1024 * 1024L * 1024L, "1 GB")]
        [InlineData(3 * 1024L * 1024L * 1024L, "3 GB")]
        [InlineData(5 * 1024L * 1024L * 1024L * 1024L, "5 TB")]
        public void TestFileSizeFormat(long bytes, string expectedOutput)
        {
            var formattedSize = _fileSizeFormatter.GetFormattedSize(bytes);

            Assert.Equal(expectedOutput, formattedSize);
        }

        [Fact]
        public void TestInvalidFileSize()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _fileSizeFormatter.GetFormattedSize(-1));
        }
    }
}