using Camelot.Services.Windows.WinApi;
using System.Diagnostics;

namespace Camelot.ViewModels.Windows.WinApi;

public static class ShellIcon
{
    public static string GetIconForExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
        {
            throw new ArgumentNullException(nameof(extension));
        }

        // But to work with Windows API, we still need to add dot prefix...
        extension = "." + extension;

        const Win32.AssocF assocFlag = Win32.AssocF.None;
        var currentAppIcon = Win32.AssocQueryString(assocFlag, Win32.AssocStr.AppIconRreference, extension);

        string result;
        if (IsValid(currentAppIcon))
        {
            result = currentAppIcon;
        }
        else
        {
            // fallback to use default
            result = Win32.AssocQueryString(assocFlag, Win32.AssocStr.DefaultIcon, extension);
        }
        return result;
    }

    private static bool IsValid(string icon)
    {
        if (string.IsNullOrEmpty(icon))
            return false;
        if (icon.TrimStart().StartsWith("%"))
        {
            // looks like because of invalid values in registry
            return false;
        }
        return true;
    }
}

