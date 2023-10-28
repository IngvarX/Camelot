using Camelot.Services.Abstractions;
using Camelot.ViewModels.Services.Interfaces;
using Camelot.ViewModels.Windows.Interfaces;

namespace Camelot.ViewModels.Windows.ShellIcons;

public class WindowsShellLinksService : IShellLinksService
{
    private readonly IPathService _pathService;
    private readonly IShellLinkResolver _resolver;

    public WindowsShellLinksService(
        IPathService pathService,
        IShellLinkResolver resolver)
    {
        _pathService = pathService;
        _resolver = resolver;
    }

    public bool IsShellLink(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        var ext = _pathService.GetExtension(path).ToLower();
        return ext == "lnk";
    }

    public string ResolveLink(string path)
    {
        if (!IsShellLink(path))
        {
            throw new ArgumentException(nameof(path));
        }

        return _resolver.ResolveLink(path);
    }
}