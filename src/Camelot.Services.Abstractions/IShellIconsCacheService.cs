using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions;

public interface IShellIconsCacheService
{
    ImageModel GetIcon(string filename);
}