using System;
using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Enums;
using Camelot.Services.Linux.Interfaces;

namespace Camelot.Services.Linux
{
    public class LinuxResourceOpeningService : IResourceOpeningService
    {
        private readonly IProcessService _processService;
        private readonly IShellCommandWrappingService _shellCommandWrappingService;
        private readonly IDesktopEnvironmentService _desktopEnvironmentService;

        private bool _isInitialized;
        private string _openCommand;
        private string _openCommandArguments;

        public LinuxResourceOpeningService(
            IProcessService processService,
            IShellCommandWrappingService shellCommandWrappingService,
            IDesktopEnvironmentService desktopEnvironmentService)
        {
            _processService = processService;
            _shellCommandWrappingService = shellCommandWrappingService;
            _desktopEnvironmentService = desktopEnvironmentService;
        }

        public void Open(string resource)
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            var arguments = GetArguments(resource);

            _processService.Run(_openCommand, arguments);
        }

        public void OpenWith(string command, string arguments, string resource)
        {
            var escapedArguments = string.Format(arguments,
                $"\\\"{resource.Replace("\"", "\\\\\\\"")}\\\"");
            var (wrappedCommand, wrappedArguments) = WrapWithNohup(command, escapedArguments);

            _processService.Run(wrappedCommand, wrappedArguments);
        }

        private void Initialize()
        {
            (_openCommand, _openCommandArguments) = GetOpenCommandAndArguments();
            _isInitialized = true;
        }

        private string GetArguments(string resource)
        {
            var escapedResource = _openCommand == "xdg-open"
                ? resource.Replace("\"", @"\\""")
                : resource.Replace("\"", @"\\\""");

            return string.Format(_openCommandArguments, escapedResource);
        }

        private (string, string) GetOpenCommandAndArguments()
        {
            var desktopEnvironment = GetDesktopEnvironment();
            switch (desktopEnvironment)
            {
                case DesktopEnvironment.Kde:
                    return WrapWithNohup("kioclient5", @"exec \""{0}\""");
                case DesktopEnvironment.Gnome:
                case DesktopEnvironment.Lxde:
                case DesktopEnvironment.Lxqt:
                case DesktopEnvironment.Mate:
                case DesktopEnvironment.Unity:
                case DesktopEnvironment.Cinnamon:
                    return WrapWithNohup("gio", @"open \""{0}\""");
                case DesktopEnvironment.Unknown:
                    return ("xdg-open", "\"{0}\"");
                default:
                    throw new ArgumentOutOfRangeException(nameof(desktopEnvironment), desktopEnvironment, null);
            }
        }

        private (string, string) WrapWithNohup(string command, string arguments) =>
            _shellCommandWrappingService.WrapWithNohup(command, arguments);

        private DesktopEnvironment GetDesktopEnvironment() =>
            _desktopEnvironmentService.GetDesktopEnvironment();
    }
}