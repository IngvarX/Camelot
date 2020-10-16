using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Archive
{
    public class ArchiveTypeMapper : IArchiveTypeMapper
    {
        private readonly IPathService _pathService;

        public ArchiveTypeMapper(
            IPathService pathService)
        {
            _pathService = pathService;
        }

        public ArchiveType? GetArchiveTypeFrom(string filePath)
        {
            var fileName = _pathService.GetFileNameWithoutExtension(filePath);
            var extension = _pathService.GetExtension(filePath);
            if (fileName.EndsWith(".tar"))
            {
                extension = "tar." + extension;
            }
        }
    }
}