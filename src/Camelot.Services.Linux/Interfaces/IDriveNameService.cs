using System.Threading.Tasks;

namespace Camelot.Services.Linux.Interfaces;

public interface IDriveNameService
{
    public Task<string> GetDriveNameAsync(string rootDirectory);
}