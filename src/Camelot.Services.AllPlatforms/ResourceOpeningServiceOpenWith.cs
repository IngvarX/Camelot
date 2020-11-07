using System.IO;
using Camelot.Services.Abstractions;

namespace Camelot.Services.AllPlatforms
{
    public class ResourceOpeningServiceOpenWith : IResourceOpeningService
    {
        private readonly IOpenWithApplicationService _openWithApplicationService;
        private readonly IResourceOpeningService _resourceOpeningService;

        public ResourceOpeningServiceOpenWith(
            IResourceOpeningService resourceOpeningService, 
            IOpenWithApplicationService openWithApplicationService)
        {
            _resourceOpeningService = resourceOpeningService;
            _openWithApplicationService = openWithApplicationService;
        }

        public void Open(string resource)
        {
            var selectedApplication = _openWithApplicationService.GetSelectedApplication(Path.GetExtension(resource));

            if (selectedApplication != null)
            {
                _resourceOpeningService.OpenWith(selectedApplication.ExecutePath, selectedApplication.Arguments,
                    resource);
            }
            else
            {
                _resourceOpeningService.Open(resource);
            }
        }

        public void OpenWith(string command, string arguments, string resource) =>
            _resourceOpeningService.OpenWith(command, arguments, resource);
    }
}
