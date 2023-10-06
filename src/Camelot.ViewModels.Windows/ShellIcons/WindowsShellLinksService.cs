using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Windows.ShellIcons;

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
        
        return ShellLink.ResolveLink(path);;
    }
}