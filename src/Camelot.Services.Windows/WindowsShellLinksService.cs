using System;
using System.IO;
using System.Runtime.Versioning;
using Camelot.Services.Abstractions;
using Camelot.Services.Windows.ShellIcons;

namespace Camelot.Services.Windows;

[SupportedOSPlatform("windows")]
public class WindowsShellLinksService : IShellLinksService
{
    public bool IsShellLink(string path)
    {
        if (string.IsNullOrEmpty(path)) 
            return false;
        var ext = Path.GetExtension(path).ToLower();
        if (ext == ".lnk")
            return true;
        return false;
    }

    public string ResolveLink(string path)
    {
        if (!IsShellLink(path))
            throw new ArgumentOutOfRangeException(nameof(path));

       var result = ShellLink.ResolveLink(path);
       return result;
    }
}