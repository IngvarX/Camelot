using ApplicationDispatcher.Interfaces;
using Microsoft.Extensions.PlatformAbstractions;

namespace ApplicationDispatcher.Implementations
{
    public class ApplicationVersionProvider : IApplicationVersionProvider
    {
        public string Version => PlatformServices.Default.Application.ApplicationVersion;
    }
}