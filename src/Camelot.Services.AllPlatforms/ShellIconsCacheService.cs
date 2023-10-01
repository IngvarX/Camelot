using System;
using System.Collections.Generic;
using Camelot.Services.Abstractions;
using System.IO;
using Camelot.Services.Abstractions.Models;
using Camelot.Images;
using Avalonia.Media.Imaging;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Environment.Enums;

namespace Camelot.Services.AllPlatforms;

public class ShellIconsCacheService : IShellIconsCacheService
{
    private readonly IShellLinksService _shellLinksService;
    private readonly IShellIconsService _shellIconsService;
    private readonly Dictionary<string, Bitmap> _cache = new();
    private readonly Platform _platform;
    public ShellIconsCacheService(
        IPlatformService platformService,
        IShellLinksService shellLinksService,
        IShellIconsService shellIconsService)
    {
        var platform = platformService.GetPlatform();
        if (platform != Platform.Windows)
            throw new InvalidOperationException($"Need to you other c'tor, without arg of {typeof(IShellLinksService)}");

        _platform = platform;
        _shellLinksService = shellLinksService;
        _shellIconsService = shellIconsService;
    }

    // The c'tor is for Mac/Linux, where cache is not implemented yet.
    public ShellIconsCacheService(
     IPlatformService platformService)
    {
        var platform = platformService.GetPlatform();
        if (platform == Platform.Windows)
            throw new InvalidOperationException($"Need to you other c'tor, with arg of {typeof(IShellLinksService)}");

        _platform = platform;
    }

    public ImageModel GetIcon(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentNullException(nameof(filename));

        var bitmap = GetShellIcon(filename);
        var result = new ConcreteImage(bitmap);
        return result;
    }

    private string ResolveIfLink(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentNullException(nameof(filename));

        string result;

        var isLink = _shellLinksService.IsShellLink(filename);
        if (isLink)
        {
            var resolved = _shellLinksService.ResolveLink(filename);
            // Check if resolved still exists,
            // sometimes the target of .lnk files
            // dont exist anymore, or links to a folder.
            if (File.Exists(resolved))
            {
                result = resolved;
            }
            else
            {
                if (Directory.Exists(resolved))
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

    private Bitmap GetShellIcon(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentNullException(nameof(filename));

        Bitmap result = null;
        string path;

        // step #1
        // resolve links, if any
        if (_platform == Platform.Windows)
        {
            path = ResolveIfLink(filename);
            if (path == null)
                return null;
        }
        else
        {
            path = filename;
        }

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
            case IShellIconsService.ShellIconType.Extension:
                {
                    var ext = Path.GetExtension(path);
                    if (string.IsNullOrEmpty(ext))
                    {
                        // a file with no extension. caller should use other icon.
                        return null;
                    }
                    else
                    {
                        if (_cache.ContainsKey(ext))
                        {
                            result = _cache[ext];
                        }
                        else
                        {
                            var image = _shellIconsService.GetIconForExtension(ext);
                            if (image != null)
                            {
                                var concreteImage = image as ConcreteImage;
                                if (concreteImage == null)
                                    throw new InvalidCastException();
                                result = concreteImage.Bitmap;
                            }
                            _cache[ext] = result;
                        }
                    }
                }
                break;
            case IShellIconsService.ShellIconType.FullPath:
                {
                    if (_cache.ContainsKey(path))
                    {
                        result = _cache[path];
                    }
                    else
                    {
                        var image = _shellIconsService.GetIconForPath(path);
                        if (image != null)
                        {
                            var concreteImage = image as ConcreteImage;
                            if (concreteImage == null)
                                throw new InvalidCastException();
                            result = concreteImage.Bitmap;
                        }
                        _cache[path] = result;
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(iconType));
        }
        
        return result;
    }
}