using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Configuration;

public class FileTypeMapperConfiguration
{
    public Dictionary<FileMimeType, string[]> FileTypeToExtensionDictionary { get; set; }

    public FileTypeMapperConfiguration()
    {
        FileTypeToExtensionDictionary = new Dictionary<FileMimeType, string[]>();
    }
}