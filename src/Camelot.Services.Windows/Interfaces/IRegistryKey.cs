using System;
using System.Collections.Generic;

namespace Camelot.Services.Windows.Interfaces
{
    public interface IRegistryKey : IDisposable
    {
        IRegistryKey OpenSubKey(string subKey);

        object GetValue(string subKey);

        IEnumerable<string> GetValueNames();

        IEnumerable<string> GetSubKeyNames();
    }
}