using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Configuration;

namespace Camelot.Services.Archive
{
    public class ArchiveTypeMapper : IArchiveTypeMapper
    {
        private readonly IPathService _pathService;
        private readonly ArchiveTypeMapperConfiguration _configuration;

        public ArchiveTypeMapper(
            IPathService pathService,
            ArchiveTypeMapperConfiguration configuration)
        {
            _pathService = pathService;
            _configuration = configuration;
        }

        public ArchiveType? GetArchiveTypeFrom(string filePath)
        {
            var fileName = _pathService.GetFileNameWithoutExtension(filePath);
            var extension = _pathService.GetExtension(filePath);
            if (fileName.EndsWith(".tar"))
            {
                extension = "tar." + extension;
            }

            return _configuration.ExtensionToArchiveTypeDictionary.TryGetValue(extension, out var result)
                ? result
                : (ArchiveType?) null;
        }
    }
}