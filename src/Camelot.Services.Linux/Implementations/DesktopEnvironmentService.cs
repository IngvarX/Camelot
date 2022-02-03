using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Enums;
using Camelot.Services.Linux.Interfaces;

namespace Camelot.Services.Linux.Implementations;

public class DesktopEnvironmentService : IDesktopEnvironmentService
{
    private readonly IEnvironmentService _environmentService;

    private DesktopEnvironment? _cachedDesktopEnvironment;

    public DesktopEnvironmentService(IEnvironmentService environmentService)
    {
        _environmentService = environmentService;
    }

    public DesktopEnvironment GetDesktopEnvironment()
    {
        _cachedDesktopEnvironment ??= LoadDesktopEnvironment();

        return _cachedDesktopEnvironment.Value;
    }

    private DesktopEnvironment LoadDesktopEnvironment()
    {
        var desktopEnvironmentName = GetDesktopEnvironmentName();

        return desktopEnvironmentName switch
        {
            "kde" => DesktopEnvironment.Kde,
            "gnome" => DesktopEnvironment.Gnome,
            "lxde" => DesktopEnvironment.Lxde,
            "lxqt" => DesktopEnvironment.Lxqt,
            "mate" => DesktopEnvironment.Mate,
            "unity" => DesktopEnvironment.Unity,
            "cinnamon" => DesktopEnvironment.Cinnamon,
            _ => DesktopEnvironment.Unknown
        };
    }

    private string GetDesktopEnvironmentName()
    {
        var xdgCurrentDesktop = _environmentService.GetEnvironmentVariable("XDG_CURRENT_DESKTOP");
        var desktopSession = _environmentService.GetEnvironmentVariable("DESKTOP_SESSION");
        var desktopEnvironmentName =
            string.IsNullOrWhiteSpace(xdgCurrentDesktop) ? desktopSession : xdgCurrentDesktop;

        return desktopEnvironmentName.ToLower();
    }
}