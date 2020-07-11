using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Enums;
using Camelot.Services.Linux.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Linux.Tests
{
    public class LinuxTerminalServiceTests
    {
        private const string Directory = "Dir";

        [Theory]
        [InlineData(DesktopEnvironment.Kde, "konsole")]
        [InlineData(DesktopEnvironment.Cinnamon, "x-terminal-emulator")]
        [InlineData(DesktopEnvironment.Gnome, "x-terminal-emulator")]
        [InlineData(DesktopEnvironment.Lxde, "x-terminal-emulator")]
        [InlineData(DesktopEnvironment.Lxqt, "x-terminal-emulator")]
        [InlineData(DesktopEnvironment.Mate, "x-terminal-emulator")]
        [InlineData(DesktopEnvironment.Unity, "x-terminal-emulator")]
        [InlineData(DesktopEnvironment.Unknown, "x-terminal-emulator")]
        public void TestOpening(DesktopEnvironment desktopEnvironment, string command)
        {
            var args = $@"--workdir \""{Directory}\""";
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(m => m.GetRepository<TerminalSettings>())
                .Returns(new Mock<IRepository<TerminalSettings>>().Object);
            var uowFactoryMock = new Mock<IUnitOfWorkFactory>();
            uowFactoryMock
                .Setup(m => m.Create())
                .Returns(uowMock.Object);
            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run(command, args))
                .Verifiable();
            var desktopEnvironmentServiceMock = new Mock<IDesktopEnvironmentService>();
            desktopEnvironmentServiceMock
                .Setup(m => m.GetDesktopEnvironment())
                .Returns(desktopEnvironment);
            var shellCommandWrappingServiceMock = new Mock<IShellCommandWrappingService>();
            shellCommandWrappingServiceMock
                .Setup(m => m.WrapWithNohup(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((c, a) => (c, a));

            var terminalService = new LinuxTerminalService(processServiceMock.Object,
                uowFactoryMock.Object, desktopEnvironmentServiceMock.Object, shellCommandWrappingServiceMock.Object);

            terminalService.Open(Directory);

            processServiceMock.Verify(m => m.Run(command, args), Times.Once);
        }
    }
}