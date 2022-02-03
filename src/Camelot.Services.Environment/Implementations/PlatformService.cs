using System.Runtime.InteropServices;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Environment.Implementations;

public class PlatformService : IPlatformService
{
    public Platform GetPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return Platform.Linux;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return Platform.MacOs;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Platform.Windows;
        }

        return Platform.Unknown;
    }
}