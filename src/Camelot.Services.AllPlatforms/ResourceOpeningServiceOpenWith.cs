using Camelot.Services.Abstractions;

namespace Camelot.Services.AllPlatforms
{
    public class ResourceOpeningServiceOpenWith : IResourceOpeningService
    {
        private readonly IOpenWithApplicationService _openWithApplicationService;
        private readonly IPathService _pathService;
        private readonly IResourceOpeningService _resourceOpeningService;

        public ResourceOpeningServiceOpenWith(
            IResourceOpeningService resourceOpeningService, 
            IOpenWithApplicationService openWithApplicationService,
            IPathService pathService)
        {
            _resourceOpeningService = resourceOpeningService;
            _openWithApplicationService = openWithApplicationService;
            _pathService = pathService;
        }

        public void Open(string resource)
        {
            var extension = _pathService.GetExtension(resource);
            var selectedApplication = _openWithApplicationService.GetSelectedApplication(extension);

            if (selectedApplication is null)
            {
                _resourceOpeningService.Open(resource);
            }
            else
            {
                _resourceOpeningService.OpenWith(selectedApplication.ExecutePath, selectedApplication.Arguments,
                    resource);
            }
        }

        public void OpenWith(string command, string arguments, string resource) =>
            _resourceOpeningService.OpenWith(command, arguments, resource);
    }
}
