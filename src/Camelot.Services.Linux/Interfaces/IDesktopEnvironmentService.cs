using Camelot.Services.Linux.Enums;

namespace Camelot.Services.Linux.Interfaces;

public interface IDesktopEnvironmentService
{
    DesktopEnvironment GetDesktopEnvironment();
}