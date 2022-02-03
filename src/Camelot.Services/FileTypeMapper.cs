using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Configuration;

namespace Camelot.Services;

public class FileTypeMapper : IFileTypeMapper
{
    private readonly Dictionary<string, FileContentType> _dictionary;

    public FileTypeMapper(FileTypeMapperConfiguration configuration)
    {
        _dictionary = BuildDictionary(configuration);
    }

    public FileContentType GetFileType(string extension)
    {
        var preprocessedExtension = Preprocess(extension);

        return _dictionary.GetValueOrDefault(preprocessedExtension, FileContentType.Other);
    }
    
    private static Dictionary<string, FileContentType> BuildDictionary(FileTypeMapperConfiguration configuration) =>
        configuration
            .FileTypeToExtensionDictionary
            .SelectMany(kvp => kvp.Value.Select(v => (Key: v, Value: kvp.Key)))
            .ToDictionary(t => t.Key, t => t.Value);

    private static string Preprocess(string extension) => extension.Trim().ToLower();
}