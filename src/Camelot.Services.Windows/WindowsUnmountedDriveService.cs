using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.AllPlatforms;

namespace Camelot.Services.Windows
{
    public class WindowsUnmountedDriveService : UnmountedDriveServiceBase
    {
        protected override Task<IReadOnlyList<UnmountedDriveModel>> GetUnmountedDrivesAsync() =>
            Task.FromResult((IReadOnlyList<UnmountedDriveModel>) Array.Empty<UnmountedDriveModel>());

        public override void Mount(string drive) =>
            throw new InvalidOperationException("Mount on Windows is not supported");
    }
}