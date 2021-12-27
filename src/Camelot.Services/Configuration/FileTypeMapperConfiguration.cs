using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Configuration;

public class FileTypeMapperConfiguration
{
    public Dictionary<string, FileMimeType> ExtensionToFileTypeDictionary { get; set; }

    public FileTypeMapperConfiguration()
    {
        ExtensionToFileTypeDictionary = new Dictionary<string, FileMimeType>();
    }
}