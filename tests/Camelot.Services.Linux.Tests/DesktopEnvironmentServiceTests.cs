using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Enums;
using Moq;
using Xunit;

namespace Camelot.Services.Linux.Tests
{
    public class DesktopEnvironmentServiceTests
    {
        private const string XdgCurrentDesktop = "XDG_CURRENT_DESKTOP";
        private const string DesktopSession = "DESKTOP_SESSION";

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
            var environmentServiceMock = new Mock<IEnvironmentService>();
            environmentServiceMock
                .Setup(m => m.GetEnvironmentVariable(environmentVariable))
                .Returns(environmentName);

            var desktopEnvironmentService = new DesktopEnvironmentService(environmentServiceMock.Object);
            var actualEnvironment = desktopEnvironmentService.GetDesktopEnvironment();

            Assert.Equal(expectedEnvironment, actualEnvironment);
        }
    }
}