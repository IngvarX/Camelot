using System;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Enums;
using Camelot.Services.Linux.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Linux.Tests
{
    public class LinuxResourceOpeningServiceTests
    {
        private const string FileName = "File.txt";
        private const string Command = "Command";
        private const string Arguments = "Arguments";

        private readonly AutoMocker _autoMocker;

        public LinuxResourceOpeningServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestFileServiceOpeningLinuxDefault()
        {
            const string command = "xdg-open";
            var arguments = $"\"{FileName}\"";

            _autoMocker
                .Setup<IProcessService>(m => m.Run(command, arguments))
                .Verifiable();
            _autoMocker
                .Setup<IDesktopEnvironmentService, DesktopEnvironment>(m => m.GetDesktopEnvironment())
                .Returns(DesktopEnvironment.Unknown);

            var fileOpeningService = _autoMocker.CreateInstance<LinuxResourceOpeningService>();

            fileOpeningService.Open(FileName);

            _autoMocker
                .Verify<IProcessService>(m => m.Run(command, arguments), Times.Once);
        }

        [Theory]
        [InlineData(DesktopEnvironment.Kde, "kioclient5")]
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

            _autoMocker
                .Setup<IProcessService>(m => m.Run(command, It.IsAny<string>()))
                .Verifiable();
            _autoMocker
                .Setup<IShellCommandWrappingService, (string, string)>(m => m.WrapWithNohup(command, It.IsAny<string>()))
                .Returns((wrappedCommand, wrappedArguments));
            _autoMocker
                .Setup<IDesktopEnvironmentService, DesktopEnvironment>(m => m.GetDesktopEnvironment())
                .Returns(desktopEnvironment);

            var fileOpeningService = _autoMocker.CreateInstance<LinuxResourceOpeningService>();

            fileOpeningService.Open(FileName);

            _autoMocker
                .Verify<IProcessService>(m => m.Run(wrappedCommand, wrappedArguments),
                    Times.Once);
        }

        [Fact]
        public void TestOsNotSupported()
        {
            _autoMocker
                .Setup<IDesktopEnvironmentService, DesktopEnvironment>(m => m.GetDesktopEnvironment())
                .Returns((DesktopEnvironment) 42);

            var fileOpeningService = _autoMocker.CreateInstance<LinuxResourceOpeningService>();

            void Open() => fileOpeningService.Open(FileName);

            Assert.Throws<ArgumentOutOfRangeException>(Open);
        }

        [Theory]
        [InlineData("gedit", "{0}", "file.txt", "gedit", "\\\"file.txt\\\"")]
        [InlineData("gedit", "{0}", "'file.txt", "gedit", "\\\"'file.txt\\\"")]
        [InlineData("gedit", "{0}", "\"file.txt", "gedit", "\\\"\\\\\\\"file.txt\\\"")]
        public void TestOpenWith(string command, string arguments, string resource,
            string commandToWrap, string argumentsToWrap)
        {
            _autoMocker
                .Setup<IProcessService>(m => m.Run(Command, Arguments))
                .Verifiable();
            _autoMocker
                .Setup<IShellCommandWrappingService, (string, string)>(m => m.WrapWithNohup(commandToWrap, argumentsToWrap))
                .Returns((Command, Arguments));

            var fileOpeningService = _autoMocker.CreateInstance<LinuxResourceOpeningService>();

            fileOpeningService.OpenWith(command, arguments, resource);

            _autoMocker
                .Verify<IProcessService>(m => m.Run(Command, Arguments),
                    Times.Once);
        }
    }
}