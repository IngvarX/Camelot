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
            FileTypeToExtensionDictionary = new Dictionary<FileMimeType, string[]>
            {
                [FileMimeType.Audio] = new[] {"mp3", "m4a"},
                [FileMimeType.Video] = new[] {"mp4"},
                [FileMimeType.Image] = new[] {"gif", "png", "jpg"}
            }
        };
        _fileTypeMapper = new FileTypeMapper(config);
    }

    [Theory]
    [InlineData("mp3", FileMimeType.Audio)]
    [InlineData("MP3", FileMimeType.Audio)]
    [InlineData("Mp4", FileMimeType.Video)]
    [InlineData(" gif", FileMimeType.Image)]
    [InlineData("png ", FileMimeType.Image)]
    [InlineData(" jPG  ", FileMimeType.Image)]
    [InlineData("m5a", FileMimeType.Other)]
    [InlineData("m4a", FileMimeType.Audio)]
    public void TestMapping(string extension, FileMimeType expectedType)
    {
        var actualType = _fileTypeMapper.GetFileType(extension);
        
        Assert.Equal(expectedType, actualType);
    }
}