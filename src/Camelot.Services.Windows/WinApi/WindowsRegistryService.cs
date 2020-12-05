using System;
using Camelot.Services.Windows.Enums;
using Camelot.Services.Windows.Interfaces;
using Microsoft.Win32;

namespace Camelot.Services.Windows.WinApi
{
    public class WindowsRegistryService : IRegistryService
    {
        public IRegistryKey GetRegistryKey(RootRegistryKey rootRegistryKey)
        {
            var winRegistryKey = rootRegistryKey switch
            {
                RootRegistryKey.CurrentUser => Registry.CurrentUser,
                RootRegistryKey.ClassesRoot => Registry.ClassesRoot,
                RootRegistryKey.LocalMachine => Registry.LocalMachine,
                _ => throw new ArgumentOutOfRangeException(nameof(rootRegistryKey), rootRegistryKey, null)
            };
            
            return new RegistryKey(winRegistryKey);
        }
    }
}