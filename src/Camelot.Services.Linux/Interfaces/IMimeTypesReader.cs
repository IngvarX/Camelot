using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Camelot.Services.Linux.Interfaces
{
    public interface IMimeTypesReader
    {
        Task<IReadOnlyDictionary<string, List<string>>> ReadAsync(Stream stream);
    }
}