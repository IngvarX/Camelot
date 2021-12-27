using System.Collections.Generic;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Configuration;

namespace Camelot.Services;

public class FileTypeMapper : IFileTypeMapper
{
    private readonly FileTypeMapperConfiguration _configuration;

    public FileTypeMapper(FileTypeMapperConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public FileMimeType GetFileType(string extension)
    {
        var preprocessedExtension = Preprocess(extension);

        return _configuration
            .ExtensionToFileTypeDictionary
            .GetValueOrDefault(preprocessedExtension, FileMimeType.Other);
    }

    private static string Preprocess(string extension) => extension.Trim().ToLower();
}