using Avalonia.Media.Imaging;
using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Interfaces;
using Camelot.ViewModels.Services.Interfaces;
using Camelot.ViewModels.Services.Interfaces.Enums;
using Camelot.ViewModels.Services.Interfaces.Models;

namespace Camelot.ViewModels.Windows.ShellIcons;

public class ShellIconsCacheService : IShellIconsCacheService
{
    private readonly IShellLinksService _shellLinksService;
    private readonly IShellIconsService _shellIconsService;
    private readonly IFileService _fileService;
    private readonly IDirectoryService _directoryService;
    private readonly IPathService _pathService;
    private readonly Dictionary<string, IBitmap> _cache;
    private readonly Platform _platform;

    public ShellIconsCacheService(
        IPlatformService platformService,
        IShellLinksService shellLinksService,
        IShellIconsService shellIconsService,
        IFileService fileService,
        IDirectoryService directoryService,
        IPathService pathService)
    {
        var platform = platformService.GetPlatform();
        if (platform != Platform.Windows)
        {
            throw new InvalidOperationException($"Need to you other c'tor, without arg of {typeof(IShellLinksService)}");
        }

        _shellLinksService = shellLinksService;
        _shellIconsService = shellIconsService;
        _fileService = fileService;
        _directoryService = directoryService;
        _pathService = pathService;

        _cache = new Dictionary<string, IBitmap>();
        _platform = platform;
    }

    // The c'tor is for Mac/Linux, where cache is not implemented yet.
    public ShellIconsCacheService(
     IPlatformService platformService)
    {
        _platform = platformService.GetPlatform();
    }

    public ImageModel GetIcon(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException(nameof(filename));
        }

        var bitmap = GetShellIcon(filename);

        return new ImageModel(bitmap);
    }

    private string ResolveIfLink(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException(nameof(filename));
        }

        string result;

        var isLink = _shellLinksService.IsShellLink(filename);
        if (isLink)
        {
            var resolved = _shellLinksService.ResolveLink(filename);
            // Check if resolved still exists,
            // sometimes the target of .lnk files
            // dont exist anymore, or links to a folder.
            if (_fileService.CheckIfExists(resolved))
            {
                result = resolved;
            }
            else
            {
                if (_directoryService.CheckIfExists(resolved))
                {
                    // resolved is folder.
                    // TODO: need to add support for icons for folders. (iksi4prs: planned in future PR).
                    result = null;
                }
                else
                {
                    // target file not found
                    result = null;
                }
            }
        }
        else
        {
            result = filename;
        }

        return result;
    }

    private IBitmap GetShellIcon(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException(nameof(filename));
        }

        string path;

        // step #1
        // resolve links, if any
        if (_platform == Platform.Windows)
        {
            path = ResolveIfLink(filename);
            if (path is null)
            {
                return null;
            }
        }
        else
        {
            path = filename;
        }

        return ShellIcon(path);
    }

    private IBitmap ShellIcon(string path)
    {
        IBitmap result;

        // step #2
        // check if cache, and if not, get from shell.
        // IMPORTANT:
        // keys in cache are both extensions only, and full paths,
        // based on result returned from shell.
        // eg, on Windows all .txt files will have same shell icon,
        // but each .exe will have its own icon (if was embdded in resource of .exe)
        var iconType = _shellIconsService.GetIconType(path);
        switch (iconType)
        {
            case ShellIconType.Extension:
            {
                var ext = _pathService.GetExtension(path);
                if (string.IsNullOrEmpty(ext))
                {
                    // a file with no extension. caller should use other icon.
                    return null;
                }

                if (_cache.TryGetValue(ext, out var value))
                {
                    result = value;
                }
                else
                {
                    var image = _shellIconsService.GetIconForExtension(ext);
                    _cache[ext] = result = image.Bitmap;
                }

                break;
            }
            case ShellIconType.FullPath:
            {
                if (_cache.TryGetValue(path, out var value))
                {
                    result = value;
                }
                else
                {
                    var image = _shellIconsService.GetIconForPath(path);
                    _cache[path] = result = image.Bitmap;
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(iconType), iconType, null);
        }

        return result;
    }
}