using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs.Archives;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class ArchiveTypeViewModelFactory : IArchiveTypeViewModelFactory
    {
        private readonly ArchiveTypeViewModelFactoryConfiguration _configuration;

        public ArchiveTypeViewModelFactory(
            ArchiveTypeViewModelFactoryConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IReadOnlyList<ArchiveTypeViewModel> CreateForSingleFile() =>
            _configuration
                .SingleFileArchiveTypes
                .Select(CreateFrom)
                .ToArray();

        public IReadOnlyList<ArchiveTypeViewModel> CreateForMultipleFiles() =>
            _configuration
                .MultipleFilesArchiveTypes
                .Select(CreateFrom)
                .ToArray();

        private static ArchiveTypeViewModel CreateFrom(KeyValuePair<ArchiveType, string> kvp) =>
            new ArchiveTypeViewModel(kvp.Key, kvp.Value);
    }
}