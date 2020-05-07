using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Linux
{
    public class LinuxResourceOpeningService : IResourceOpeningService
    {
        private readonly IProcessService _processService;
        private readonly string _openCommand;
        private readonly string _openCommandArguments;

        public LinuxResourceOpeningService(
            IProcessService processService,
            IEnvironmentService environmentService)
        {
            _processService = processService;

            (_openCommand, _openCommandArguments) = GetOpenCommandAndArguments(environmentService);
        }

        public void Open(string resource)
        {
            var arguments = GetArguments(resource);

            _processService.Run(_openCommand, arguments);
        }

        private string GetArguments(string resource)
        {
            var escapedResource = _openCommand == "xdg-open"
                ? resource.Replace("\"", @"\\""")
                : resource.Replace("\"", @"\\\""");

            return string.Format(_openCommandArguments, escapedResource);
        }

        // TODO: refactor
        private static (string, string) GetOpenCommandAndArguments(IEnvironmentService environmentService)
        {
            var desktopEnvironmentName = GetDesktopEnvironmentName(environmentService);
            switch (desktopEnvironmentName)
            {
                case "kde":
                    return WrapWithNohup("kioclient", @"exec \""{0}\""");
                case "gnome":
                case "lxde":
                case "lxqt":
                case "mate":
                case "unity":
                case "cinnamon":
                    return WrapWithNohup("gio", @"open \""{0}\""");
                default:
                    return ("xdg-open", "\"{0}\"");
            }
        }

        private static (string, string) WrapWithNohup(string command, string arguments) =>
            ("bash", $"-c \"nohup {command} {arguments} >/dev/null 2>&1\"");

        private static string GetDesktopEnvironmentName(IEnvironmentService environmentService)
        {
            var xdgCurrentDesktop = environmentService.GetEnvironmentVariable("XDG_CURRENT_DESKTOP");
            var desktopSession = environmentService.GetEnvironmentVariable("DESKTOP_SESSION");
            var desktopEnvironmentName =
                string.IsNullOrWhiteSpace(xdgCurrentDesktop) ? desktopSession : xdgCurrentDesktop;

            return desktopEnvironmentName.ToLower();
        }
    }
}