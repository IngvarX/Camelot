using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Mac
{
    public class MacUnmountedDriveService : IUnmountedDriveService
    {
        // TODO: load drives
        public Task<IReadOnlyList<UnmountedDriveModel>> GetUnmountedDrivesAsync() =>
            Task.FromResult((IReadOnlyList<UnmountedDriveModel>) Array.Empty<UnmountedDriveModel>());
    }
}