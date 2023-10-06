using Camelot.ViewModels.Services.Interfaces.Enums;
using Camelot.ViewModels.Services.Interfaces.Models;

namespace Camelot.ViewModels.Services.Interfaces;

public interface IShellIconsService
{
    // Motivation for 3 methods:
    // There are 3 methods and not only simple one "getter",
    // in order to maintain a cache in the Avalonia level,
    // so cached images are of the final type which is use for rendering,
    // namely Avalonia and not System.Drawing (or other platform dependent)
    // so cache will be more efficient.
    // Using information provided by methods below,
    // the Avalonia cache "knows" which key to use for cache.

    ShellIconType GetIconType(string filename);
    
    ImageModel GetIconForPath(string path);
    
    ImageModel GetIconForExtension(string extension);
}