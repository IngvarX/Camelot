using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Configuration
{
    public class ArchiveTypeMapperConfiguration
    {
        public Dictionary<string, ArchiveType> ExtensionToArchiveTypeDictionary { get; set; }

        public ArchiveTypeMapperConfiguration()
        {
            ExtensionToArchiveTypeDictionary = new Dictionary<string, ArchiveType>();
        }
    }
}