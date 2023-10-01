using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Windows.ShellIcons;
using Camelot.Images;

using SystemBitmap = System.Drawing.Bitmap;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;


namespace Camelot.Services.Windows;


[SupportedOSPlatform("windows")]
public class WindowsShellIconsService : IShellIconsService
{
    public ImageModel GetIconForExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            throw new ArgumentNullException(nameof(extension));
        if (!extension.StartsWith("."))
            throw new ArgumentOutOfRangeException(nameof(extension));
        if (extension.ToLower() == ".lnk")
            throw new ArgumentOutOfRangeException("Need to resolve .lnk first");

        var iconFilename = ShellIcon.GetIconForExtension(extension);

        if (string.IsNullOrEmpty(iconFilename))
        {
            // shell has no ico for this one
            return null;
        }

        if (UWPIcon.IsPriString(iconFilename))
        {
            iconFilename = UWPIcon.ReslovePackageResource(iconFilename);
        }
        return LoadIcon(iconFilename);
    }

    public ImageModel GetIconForPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (GetIconType(path) != IShellIconsService.ShellIconType.FullPath)
            throw new ArgumentOutOfRangeException(nameof(path));

        var ext = Path.GetExtension(path).ToLower();
        if (ext == ".lnk")
           throw new ArgumentOutOfRangeException("Need to resolve .lnk first");

        return LoadIcon(path);
    }

    private ImageModel LoadIcon(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        ImageModel result;

        var needsExtract = WindowsIconTypes.IsIconThatRequiresExtract(path);
        if (needsExtract)
        {
            var icon = IconExtractor.ExtractIcon(path);
            // TODO: check if lossy and/or try other options, see url below. (iksi4prs).
            // https://learn.microsoft.com/en-us/dotnet/api/system.drawing.imageconverter.canconvertfrom?view=dotnet-plat-ext-7.0
            SystemBitmap systemBitmap = icon.ToBitmap();
            AvaloniaBitmap avaloniaBitmap = SystemImageToAvaloniaBitmapConverter.Convert(systemBitmap);
            result = new ConcreteImage(avaloniaBitmap);
        }
        else
        {
            if (File.Exists(path))
            {
                AvaloniaBitmap avaloniaBitmap = new AvaloniaBitmap(path);
                result = new ConcreteImage(avaloniaBitmap);
            }
            else
            {
                // no shell icon, caller should use other icon
                result = null;
            }
            
        }
        return result;
    }

    public IShellIconsService.ShellIconType GetIconType(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentNullException(nameof(filename));
        
        var ext = Path.GetExtension(filename).ToLower();

        // next extensions require that the icon will be resolved by full path,
        // and not just the extension itself.
        var extensionForFullPaths = new string[] { ".exe", ".cpl", ".appref-ms", ".msc" };
        if (extensionForFullPaths.Contains(ext))
            return IShellIconsService.ShellIconType.FullPath;
        
        return IShellIconsService.ShellIconType.Extension;
    }
}