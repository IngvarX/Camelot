using System;
using System.Runtime.Versioning;
using Camelot.Services.Windows.WinApi;

namespace Camelot.Services.Windows.ShellIcons;

[SupportedOSPlatform("windows")]
public class ShellIcon
{
    static public string GetIconForExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            throw new ArgumentNullException(nameof(extension));
        if (!extension.StartsWith("."))
            throw new ArgumentOutOfRangeException(nameof(extension));

        var assocFlag = Win32.AssocF.None;
        var currentAppIcon = Win32.AssocQueryString(assocFlag, Win32.AssocStr.AppIconRreference, extension);
        if (!string.IsNullOrEmpty(currentAppIcon))
            return currentAppIcon;

        var defaultIcon = Win32.AssocQueryString(assocFlag, Win32.AssocStr.DefaultIcon, extension);
        return defaultIcon;
    }
}

