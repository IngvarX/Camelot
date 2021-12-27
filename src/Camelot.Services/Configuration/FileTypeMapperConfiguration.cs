using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Configuration;

public class FileTypeMapperConfiguration
{
    public Dictionary<FileContentType, string[]> FileTypeToExtensionDictionary { get; set; }

    public FileTypeMapperConfiguration()
    {
        FileTypeToExtensionDictionary = new Dictionary<FileContentType, string[]>();
    }
}