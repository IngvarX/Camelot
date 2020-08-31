using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Linux
{
    public class LinuxUnmountedDriveService : IUnmountedDriveService
    {
        // TODO: add implementation
        public IEnumerable<UnmountedDriveModel> GetUnmountedDrives() =>
            Enumerable.Empty<UnmountedDriveModel>();
    }
}