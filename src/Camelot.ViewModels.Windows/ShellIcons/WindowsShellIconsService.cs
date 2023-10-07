using Camelot.ViewModels.Services.Interfaces;
using Camelot.ViewModels.Services.Interfaces.Enums;
using Camelot.ViewModels.Services.Interfaces.Models;
using SystemBitmap = System.Drawing.Bitmap;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;


namespace Camelot.ViewModels.Windows.ShellIcons;


public class WindowsShellIconsService : IShellIconsService
{
    public ImageModel GetIconForExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
        {
            throw new ArgumentNullException(nameof(extension));
        }

        if (!extension.StartsWith("."))
        {
            throw new ArgumentException(nameof(extension));
        }

        if (extension.ToLower() == ".lnk")
        {
            throw new ArgumentException("Need to resolve .lnk first");
        }

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

        if (GetIconType(path) != ShellIconType.FullPath)
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
            var systemBitmap = icon.ToBitmap();
            var avaloniaBitmap = SystemImageToAvaloniaBitmapConverter.Convert(systemBitmap);
            result = new ImageModel(avaloniaBitmap);
        }
        else
        {
            if (File.Exists(path))
            {
                var avaloniaBitmap = new AvaloniaBitmap(path);
                result = new ImageModel(avaloniaBitmap);
            }
            else
            {
                // no shell icon, caller should use other icon
                result = null;
            }

        }
        return result;
    }

    public ShellIconType GetIconType(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentNullException(nameof(filename));

        var ext = Path.GetExtension(filename).ToLower();

        // next extensions require that the icon will be resolved by full path,
        // and not just the extension itself.
        var extensionForFullPaths = new[] { ".exe", ".cpl", ".appref-ms", ".msc" };

        return extensionForFullPaths.Contains(ext)
            ? ShellIconType.FullPath
            : ShellIconType.Extension;
    }
}