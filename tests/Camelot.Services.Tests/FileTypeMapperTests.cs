using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Configuration;
using Xunit;

namespace Camelot.Services.Tests;

public class FileTypeMapperTests
{
    private readonly FileTypeMapper _fileTypeMapper;

    public FileTypeMapperTests()
    {
        var config = new FileTypeMapperConfiguration
        {
            FileTypeToExtensionDictionary = new Dictionary<FileContentType, string[]>
            {
                [FileContentType.Audio] = new[] {"mp3", "m4a"},
                [FileContentType.Video] = new[] {"mp4"},
                [FileContentType.Image] = new[] {"gif", "png", "jpg"}
            }
        };
        _fileTypeMapper = new FileTypeMapper(config);
    }

    [Theory]
    [InlineData("mp3", FileContentType.Audio)]
    [InlineData("MP3", FileContentType.Audio)]
    [InlineData("Mp4", FileContentType.Video)]
    [InlineData(" gif", FileContentType.Image)]
    [InlineData("png ", FileContentType.Image)]
    [InlineData(" jPG  ", FileContentType.Image)]
    [InlineData("m5a", FileContentType.Other)]
    [InlineData("m4a", FileContentType.Audio)]
    public void TestMapping(string extension, FileContentType expectedType)
    {
        var actualType = _fileTypeMapper.GetFileType(extension);
        
        Assert.Equal(expectedType, actualType);
    }
}