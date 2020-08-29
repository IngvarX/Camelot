using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Windows
{
    public class WindowsHomeDirectoryProvider : IHomeDirectoryProvider
    {
        public string HomeDirectoryPath { get; }

        public WindowsHomeDirectoryProvider(IEnvironmentService environmentService)
        {
            HomeDirectoryPath = environmentService.GetEnvironmentVariable("USERPROFILE");
        }
    }
}