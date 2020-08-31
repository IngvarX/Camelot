using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Windows
{
    public class WindowsUnmountedDriveService : IUnmountedDriveService
    {
        public IEnumerable<UnmountedDriveModel> GetUnmountedDrives() =>
            Enumerable.Empty<UnmountedDriveModel>();
    }
}