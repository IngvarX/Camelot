using System;
using System.Runtime.InteropServices;
using Camelot.Services.Enums;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
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

            throw new NotSupportedException("Unsupported platform");
        }
    }
}