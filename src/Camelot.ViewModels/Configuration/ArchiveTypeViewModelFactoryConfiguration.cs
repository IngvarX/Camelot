using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.ViewModels.Configuration
{
    public class ArchiveTypeViewModelFactoryConfiguration
    {
        public Dictionary<ArchiveType, string> SingleFileArchiveTypes { get; set; }

        public Dictionary<ArchiveType, string> MultipleFilesArchiveTypes { get; set; }
    }
}