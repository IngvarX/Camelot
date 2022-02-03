using System;
using Camelot.Services.Windows.Builders;
using Xunit;

namespace Camelot.Services.Windows.Tests;

public class WindowsRemovedFileMetadataBuilderTests
{
    [Fact]
    public void TestMetadataBuilder()
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
}