using Camelot.Services.Abstractions;

namespace Camelot.Services
{
    public class FileNameGenerationService : IFileNameGenerationService
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;

        public FileNameGenerationService(
            IFileService fileService,
            IDirectoryService directoryService,
            IPathService pathService)
        {
            _fileService = fileService;
            _directoryService = directoryService;
            _pathService = pathService;
        }

        public string GenerateFullName(string filePath)
        {
            var initialName = _pathService.GetFileName(filePath);

            return GenerateByInitialName(filePath, initialName);
        }

        public string GenerateFullNameWithoutExtension(string filePath)
        {
            var initialName = _pathService.GetFileNameWithoutExtension(filePath);

            return GenerateByInitialName(filePath, initialName);
        }

        public string GenerateName(string initialName, string directory)
        {
            var currentName = initialName;
            for (var i = 1; IsNameAlreadyInUse(currentName, directory); i++)
            {
                currentName = GenerateNewName(initialName, i);
            }

            return currentName;
        }

        private string GenerateByInitialName(string filePath, string initialName)
        {
            var directory = _pathService.GetParentDirectory(filePath);
            var newName = GenerateName(initialName, directory);

            return _pathService.Combine(directory, newName);
        }

        private bool IsNameAlreadyInUse(string name, string directory)
        {
            var fullPath = _pathService.Combine(directory, name);

            return _fileService.CheckIfExists(fullPath) || _directoryService.CheckIfExists(fullPath);
        }

        private string GenerateNewName(string currentName, int i)
        {
            var fileName = _pathService.GetFileNameWithoutExtension(currentName);
            var extension = _pathService.GetExtension(currentName);

            return string.IsNullOrEmpty(extension) ? $"{fileName} ({i})" : $"{fileName} ({i}).{extension}";
        }
    }
}