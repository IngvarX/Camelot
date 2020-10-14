using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface ISoftwareService
    {
        Task<IEnumerable<SoftwareModel>> GetAllInstalledSoftwares();
    }
}
