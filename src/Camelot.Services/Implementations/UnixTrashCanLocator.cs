using System.Collections.Generic;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class UnixTrashCanLocator : ITrashCanLocator
    {
        private readonly IEnvironmentService _environmentService;
        private readonly IDriveService _driveService;

        public UnixTrashCanLocator(
            IEnvironmentService environmentService,
            IDriveService driveService)
        {
            _environmentService = environmentService;
            _driveService = driveService;
        }
        
        public IReadOnlyCollection<string> GetTrashCanDirectories(string volume)
        {
            var directories = new List<string>();
            if (volume != "/")
            {
                directories.AddRange(GetVolumeTrashCanPaths(volume));
            }
            
            directories.Add(GetHomeTrashCanPath());

            return directories;
        }

        private string GetHomeTrashCanPath()
        {
            var xdgDataHome = _environmentService.GetEnvironmentVariable("XDG_DATA_HOME");
            if (xdgDataHome != null)
            {
                return $"{xdgDataHome}/Trash/";
            }
            
            var home = _environmentService.GetEnvironmentVariable("HOME");

            return $"{home}/.local/share/Trash";
        }

        private IReadOnlyCollection<string> GetVolumeTrashCanPaths(string volume)
        {
            var uid = GetUid();

            return new[] {$"{volume}/.Trash/{uid}", $"{volume}/.Trash-{uid}"};
        }

        private string GetUid() => _environmentService.GetEnvironmentVariable("UID");
    }
}