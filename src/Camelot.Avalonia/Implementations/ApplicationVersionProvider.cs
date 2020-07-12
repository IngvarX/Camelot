using Camelot.Avalonia.Interfaces;
using Microsoft.Extensions.PlatformAbstractions;

namespace Camelot.Avalonia.Implementations
{
    public class ApplicationVersionProvider : IApplicationVersionProvider
    {
        public string Version => PlatformServices.Default.Application.ApplicationVersion;
    }
}