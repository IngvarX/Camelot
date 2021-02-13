using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.AllPlatforms;

namespace Camelot.Services.Windows
{
    public class WindowsUnmountedDriveService : UnmountedDriveServiceBase
    {
        public override void Mount(string drive) =>
            throw new InvalidOperationException("Mount on Windows is not supported");

        protected override Task<IReadOnlyList<UnmountedDriveModel>> GetUnmountedDrivesAsync() =>
            Task.FromResult((IReadOnlyList<UnmountedDriveModel>) Array.Empty<UnmountedDriveModel>());
    }
}