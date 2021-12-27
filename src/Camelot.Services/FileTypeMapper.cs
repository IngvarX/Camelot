using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Configuration;

namespace Camelot.Services;

public class FileTypeMapper : IFileTypeMapper
{
    private readonly Dictionary<string, FileMimeType> _dictionary;

    public FileTypeMapper(FileTypeMapperConfiguration configuration)
    {
        _dictionary = BuildDictionary(configuration);
    }

    public FileMimeType GetFileType(string extension)
    {
        var preprocessedExtension = Preprocess(extension);

        return _dictionary.GetValueOrDefault(preprocessedExtension, FileMimeType.Other);
    }
    
    private static Dictionary<string, FileMimeType> BuildDictionary(FileTypeMapperConfiguration configuration) =>
        configuration
            .FileTypeToExtensionDictionary
            .SelectMany(kvp => kvp.Value.Select(v => (v, kvp.Key)))
            .ToDictionary(t => t.v, t => t.Key);

    private static string Preprocess(string extension) => extension.Trim().ToLower();
}