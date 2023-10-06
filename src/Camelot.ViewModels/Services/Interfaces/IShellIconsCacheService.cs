using Camelot.ViewModels.Services.Interfaces.Models;

namespace Camelot.ViewModels.Services.Interfaces;

public interface IShellIconsCacheService
{
    ImageModel GetIcon(string filename);
}