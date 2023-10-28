using Camelot.Services.Windows.WinApi;

namespace Camelot.ViewModels.Windows.WinApi;

public static class ShellIcon
{
    public static string GetIconForExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
        {
            throw new ArgumentNullException(nameof(extension));
        }

        if (extension.StartsWith("."))
        {
            // As per stanards of this project, extension don't have dot prefix.
            throw new ArgumentOutOfRangeException(nameof(extension));
        }

        // But to work with Windows API, we still need to add dot prefix...
        extension = "." + extension;

        const Win32.AssocF assocFlag = Win32.AssocF.None;
        var currentAppIcon = Win32.AssocQueryString(assocFlag, Win32.AssocStr.AppIconRreference, extension);

        return string.IsNullOrEmpty(currentAppIcon)
            ? Win32.AssocQueryString(assocFlag, Win32.AssocStr.DefaultIcon, extension)
            : currentAppIcon;
    }
}

