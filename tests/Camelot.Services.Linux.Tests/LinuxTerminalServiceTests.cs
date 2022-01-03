using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Enums;
using Camelot.Services.Linux.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Linux.Tests;

public class LinuxTerminalServiceTests
{
    private const string Directory = "Dir";

    private readonly AutoMocker _autoMocker;

    public LinuxTerminalServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

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
        _autoMocker
            .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
            .Returns(uowMock.Object);
        _autoMocker
            .Setup<IProcessService>(m => m.Run(command, args))
            .Verifiable();
        _autoMocker
            .Setup<IDesktopEnvironmentService, DesktopEnvironment>(m => m.GetDesktopEnvironment())
            .Returns(desktopEnvironment);
        _autoMocker
            .Setup<IShellCommandWrappingService, (string, string)>(m => m.WrapWithNohup(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((c, a) => (c, a));

        var terminalService = _autoMocker.CreateInstance<LinuxTerminalService>();

        terminalService.Open(Directory);

        _autoMocker.Verify<IProcessService>(m => m.Run(command, args), Times.Once);
    }
}