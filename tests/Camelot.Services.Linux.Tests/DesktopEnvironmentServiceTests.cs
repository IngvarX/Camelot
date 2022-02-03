using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Enums;
using Camelot.Services.Linux.Implementations;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Linux.Tests;

public class DesktopEnvironmentServiceTests
{
    private const string XdgCurrentDesktop = "XDG_CURRENT_DESKTOP";
    private const string DesktopSession = "DESKTOP_SESSION";

    private readonly AutoMocker _autoMocker;

    public DesktopEnvironmentServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData("kde", DesktopEnvironment.Kde, XdgCurrentDesktop)]
    [InlineData("gnome", DesktopEnvironment.Gnome, DesktopSession)]
    [InlineData("unity", DesktopEnvironment.Unity, DesktopSession)]
    [InlineData("mate", DesktopEnvironment.Mate, DesktopSession)]
    [InlineData("unknown", DesktopEnvironment.Unknown, XdgCurrentDesktop)]
    [InlineData("cinnamon", DesktopEnvironment.Cinnamon, DesktopSession)]
    [InlineData("unity", DesktopEnvironment.Unity, XdgCurrentDesktop)]
    [InlineData("lxde", DesktopEnvironment.Lxde, DesktopSession)]
    [InlineData("lxqt", DesktopEnvironment.Lxqt, XdgCurrentDesktop)]
    public void TestDesktopEnvironment(string environmentName, DesktopEnvironment expectedEnvironment,
        string environmentVariable)
    {
        _autoMocker
            .Setup<IEnvironmentService, string>(m => m.GetEnvironmentVariable(environmentVariable))
            .Returns(environmentName);

        var desktopEnvironmentService = _autoMocker.CreateInstance<DesktopEnvironmentService>();
        var actualEnvironment = desktopEnvironmentService.GetDesktopEnvironment();

        Assert.Equal(expectedEnvironment, actualEnvironment);
    }
}