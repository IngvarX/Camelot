using Camelot.Services.Windows.Interfaces;

namespace Camelot.Services.Windows.WinApi
{
    public class WindowsApplicationInfoProvider : IApplicationInfoProvider
    {
        public (string Name, string StartCommand, string ExecutePath) GetInfo(string applicationFile)
        {
            var assocFlag = Win32.AssocF.None;
            if (applicationFile.Contains(".exe"))
            {
                assocFlag = Win32.AssocF.OpenByExeName;
            }

            var displayName = Win32.AssocQueryString(assocFlag, Win32.AssocStr.FriendlyAppName, applicationFile);
            var startCommand = Win32.AssocQueryString(assocFlag, Win32.AssocStr.Command, applicationFile);

            if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(startCommand))
            {
                return default;
            }
            
            var executePath = Win32.AssocQueryString(assocFlag, Win32.AssocStr.Executable, applicationFile);

            return (displayName, startCommand, executePath);
        }
    }
}