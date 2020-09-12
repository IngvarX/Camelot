using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface IUnmountedDriveService
    {
        Task<IReadOnlyList<UnmountedDriveModel>> GetUnmountedDrivesAsync();

        void Mount(string drive);
    }
}