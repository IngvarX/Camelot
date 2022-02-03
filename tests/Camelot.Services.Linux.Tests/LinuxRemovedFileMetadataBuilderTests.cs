using System;
using Camelot.Services.Linux.Builders;
using Xunit;

namespace Camelot.Services.Linux.Tests;

public class LinuxRemovedFileMetadataBuilderTests
{
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