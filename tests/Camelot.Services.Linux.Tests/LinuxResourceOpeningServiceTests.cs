using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Enums;
using Camelot.Services.Linux.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Linux.Tests
{
    public class LinuxResourceOpeningServiceTests
    {
        private const string FileName = "File.txt";

        [Fact]
        public void TestFileServiceOpeningLinuxDefault()
        {
            const string command = "xdg-open";
            var arguments = $"\"{FileName}\"";

            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run(command, arguments))
                .Verifiable();
            var shellCommandWrappingService = new Mock<IShellCommandWrappingService>();
            var desktopEnvironmentService = new Mock<IDesktopEnvironmentService>();
            desktopEnvironmentService
                .Setup(m => m.GetDesktopEnvironment())
                .Returns(DesktopEnvironment.Unknown);

            var fileOpeningService = new LinuxResourceOpeningService(
                processServiceMock.Object,
                shellCommandWrappingService.Object,
                desktopEnvironmentService.Object);

            fileOpeningService.Open(FileName);

            processServiceMock.Verify(m => m.Run(command, arguments), Times.Once());
        }

        [Theory]
        [InlineData(DesktopEnvironment.Kde, "kioclient")]
        [InlineData(DesktopEnvironment.Gnome, "gio")]
        [InlineData(DesktopEnvironment.Lxde, "gio")]
        [InlineData(DesktopEnvironment.Lxqt, "gio")]
        [InlineData(DesktopEnvironment.Mate, "gio")]
        [InlineData(DesktopEnvironment.Unity, "gio")]
        [InlineData(DesktopEnvironment.Cinnamon, "gio")]
        public void TestFileServiceOpeningLinux(DesktopEnvironment desktopEnvironment, string command)
        {
            const string wrappedCommand = "wrappedCommand";
            const string wrappedArguments = "wrappedArguments";

            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run(command, It.IsAny<string>()))
                .Verifiable();
            var shellCommandWrappingService = new Mock<IShellCommandWrappingService>();
            shellCommandWrappingService
                .Setup(m => m.WrapWithNohup(command, It.IsAny<string>()))
                .Returns((wrappedCommand, wrappedArguments));
            var desktopEnvironmentService = new Mock<IDesktopEnvironmentService>();
            desktopEnvironmentService
                .Setup(m => m.GetDesktopEnvironment())
                .Returns(desktopEnvironment);

            var fileOpeningService = new LinuxResourceOpeningService(
                processServiceMock.Object,
                shellCommandWrappingService.Object,
                desktopEnvironmentService.Object);

            fileOpeningService.Open(FileName);

            processServiceMock.Verify(m => m.Run(wrappedCommand, wrappedArguments), Times.Once());
        }
    }
}