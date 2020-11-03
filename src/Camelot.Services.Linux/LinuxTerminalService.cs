using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions.Models.State;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Enums;
using Camelot.Services.Linux.Interfaces;

namespace Camelot.Services.Linux
{
    public class LinuxTerminalService : TerminalServiceBase
    {
        private readonly IDesktopEnvironmentService _desktopEnvironmentService;
        private readonly IShellCommandWrappingService _shellCommandWrappingService;

        public LinuxTerminalService(
            IProcessService processService,
            IUnitOfWorkFactory unitOfWorkFactory,
            IDesktopEnvironmentService desktopEnvironmentService,
            IShellCommandWrappingService shellCommandWrappingService)
            : base(processService, unitOfWorkFactory)
        {
            _desktopEnvironmentService = desktopEnvironmentService;
            _shellCommandWrappingService = shellCommandWrappingService;
        }

        protected override TerminalSettingsStateModel GetDefaultSettings()
        {
            var desktopEnvironment = _desktopEnvironmentService.GetDesktopEnvironment();
            var (command, arguments) = desktopEnvironment switch
            {
                DesktopEnvironment.Kde => ("konsole", @"--workdir \""{0}\"""),
                _ => ("x-terminal-emulator", @"--workdir \""{0}\""")
            };

            return CreateFrom(command, arguments);
        }

        protected override (string, string) Wrap(string command, string arguments) =>
            _shellCommandWrappingService.WrapWithNohup(command, arguments);

        protected override string Escape(string directory) =>
            directory.Replace("\"", @"\\\""");

        private static TerminalSettingsStateModel CreateFrom(string command, string arguments) =>
            new TerminalSettingsStateModel {Command = command, Arguments = arguments};
    }
}