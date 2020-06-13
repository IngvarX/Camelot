using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
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

        protected override TerminalSettings GetDefaultCommand()
        {
            var desktopEnvironment = _desktopEnvironmentService.GetDesktopEnvironment();
            var (command, arguments) = desktopEnvironment switch
            {
                DesktopEnvironment.Kde => ("konsole", "--workdir {0}"),
                _ => ("x-terminal-emulator", "--workdir {0}")
            };

            return new TerminalSettings {Command = command, Arguments = arguments};
        }

        protected override (string, string) Wrap(string command, string arguments) =>
            _shellCommandWrappingService.WrapWithNohup(command, arguments);
    }
}