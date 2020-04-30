using System;
using Camelot.Services.Builders;
using Xunit;

namespace Camelot.Services.Tests
{
    public class RemovedFileMetadataBuildersTests
    {
        [Fact]
        public void TestWindowsMetadataBuilder()
        {
            const string filePath = "C://test.txt";
            var builder = new WindowsRemovedFileMetadataBuilder()
                .WithFilePath(filePath)
                .WithRemovingDateTime(DateTime.Now)
                .WithFileSize(42);

            var metadata = builder.Build();
            
            Assert.NotNull(metadata);
            Assert.Equal(30 + filePath.Length * 2, metadata.Length);
        }
        
        [Fact]
        public void TestLinuxMetadataBuilder()
        {
            const string filePath = "/home/test/file.txt";
            var now = DateTime.Now;
            var builder = new LinuxRemovedFileMetadataBuilder()
                .WithFilePath(filePath)
                .WithRemovingDateTime(now);

            var metadata = builder.Build();
            
            Assert.NotNull(metadata);
            Assert.Equal(4, metadata.Split("\n").Length);
            Assert.Contains(filePath, metadata);
            Assert.Contains(now.ToString("s"), metadata);
        }
    }
}