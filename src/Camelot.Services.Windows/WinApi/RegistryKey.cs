using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Windows.Interfaces;
using WinRegistryKey = Microsoft.Win32.RegistryKey;

namespace Camelot.Services.Windows.WinApi
{
    public class RegistryKey : IRegistryKey
    {
        private readonly WinRegistryKey _winRegistryKey;

        public RegistryKey(WinRegistryKey winRegistryKey)
        {
            _winRegistryKey = winRegistryKey;
        }

        public IRegistryKey OpenSubKey(string subKey) => new RegistryKey(_winRegistryKey?.OpenSubKey(subKey));
        
        public object GetValue(string subKey) => _winRegistryKey?.GetValue(subKey);
        
        public IEnumerable<string> GetValueNames() => _winRegistryKey?.GetValueNames()  ?? Enumerable.Empty<string>();
        
        public IEnumerable<string> GetSubKeyNames() => _winRegistryKey?.GetSubKeyNames() ?? Enumerable.Empty<string>();

        public void Dispose() => _winRegistryKey?.Dispose();
    }
}