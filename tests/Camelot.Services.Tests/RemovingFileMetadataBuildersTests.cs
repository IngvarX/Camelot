using System;
using Camelot.Services.Builders;
using Xunit;

namespace Camelot.Services.Tests
{
    public class RemovingFileMetadataBuildersTests
    {
        [Fact]
        public void TestWindowsMetadataBuilder()
        {
            const string filePath = "C://test.txt";
            var builder = new WindowsRemovedFileMetadataBuilder()
                .WithFilePath(filePath)
                .WithRemovingDate(DateTime.Now)
                .WithFileSize(42);

            var metadata = builder.Build();
            
            Assert.NotNull(metadata);
            Assert.Equal(metadata.Length, 28 + filePath.Length * 2);
        }
    }
}