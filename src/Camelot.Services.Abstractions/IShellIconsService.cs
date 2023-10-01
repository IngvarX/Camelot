using Camelot.Services.Abstractions.Models;


namespace Camelot.Services.Abstractions;

public interface IShellIconsService
{
    // Motivation for 3 methods:
    // There are 3 methods and not only simple one "getter",
    // in order to maintain a cache in the Avalonia level,
    // so cached images are of the final type which is use for renderging,
    // namly Avalonia and not System.Drawing (or other platform dependent)
    // so cache will be more efficient.
    // Using information provided by methods below,
    // the Avalonia cache "knows" which key to use for cache.

    enum ShellIconType
    {
        Extension,
        FullPath,
    }
    ShellIconType GetIconType(string filename);
    ImageModel GetIconForPath(string path);
    ImageModel GetIconForExtension(string extension);
}