using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Camelot.Services.Linux.Interfaces
{
    public interface IIniReader
    {
        Task<IReadOnlyDictionary<string, string>> ReadAsync(Stream stream);
    }
}