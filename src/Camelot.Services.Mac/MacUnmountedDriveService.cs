using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Mac
{
    public class MacUnmountedDriveService : IUnmountedDriveService
    {
        // TODO: load drives
        public IEnumerable<UnmountedDriveModel> GetUnmountedDrives() =>
            Enumerable.Empty<UnmountedDriveModel>();
    }
}