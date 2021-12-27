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
            ExtensionToFileTypeDictionary = new Dictionary<string, FileMimeType>
            {
                ["mp3"] = FileMimeType.Music,
                ["m4a"] = FileMimeType.Music
            }
        };
        _fileTypeMapper = new FileTypeMapper(config);
    }

    [Theory]
    [InlineData("mp3", FileMimeType.Music)]
    [InlineData("MP3", FileMimeType.Music)]
    [InlineData(" Mp3 ", FileMimeType.Music)]
    [InlineData("m5a", FileMimeType.Other)]
    [InlineData("m4a", FileMimeType.Music)]
    public void TestMapping(string extension, FileMimeType expectedType)
    {
        var actualType = _fileTypeMapper.GetFileType(extension);
        
        Assert.Equal(expectedType, actualType);
    }
}