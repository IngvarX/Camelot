using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.AllPlatforms
{
    public class UnixHomeDirectoryProvider : IHomeDirectoryProvider
    {
        public string HomeDirectoryPath { get; }

        public UnixHomeDirectoryProvider(IEnvironmentService environmentService)
        {
            HomeDirectoryPath = environmentService.GetEnvironmentVariable("HOME");
        }
    }
}