using System.IO;
using Camelot.Services.Abstractions;
using Camelot.Services.Behaviors;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux;
using Camelot.Services.Mac;
using Camelot.Services.Windows;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class ResourcesOpeningTests
    {
        private const string FileName = "File.txt";

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

        public ResourcesOpeningTests()
        {
            if (!File.Exists(FileName))
            {
                File.WriteAllText(FileName, FileName);
            }
        }

        [Fact]
        public void TestDirectoryOpening()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .SetupSet(m => m.SelectedDirectory = CurrentDirectory)
                .Verifiable();

            var directoryOpeningBehavior = new DirectoryOpeningBehavior(directoryServiceMock.Object);
            directoryOpeningBehavior.Open(CurrentDirectory);

            directoryServiceMock.VerifySet(c => c.SelectedDirectory = CurrentDirectory, Times.Once());
        }

        [Fact]
        public void TestFileOpening()
        {
            var fileOpeningServiceMock = new Mock<IResourceOpeningService>();
            fileOpeningServiceMock
                .Setup(m => m.Open(FileName))
                .Verifiable();

            var fileOpeningBehavior = new FileOpeningBehavior(fileOpeningServiceMock.Object);
            fileOpeningBehavior.Open(FileName);

            fileOpeningServiceMock.Verify(m => m.Open(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void TestFileServiceOpeningWindows()
        {
            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run(FileName))
                .Verifiable();

            var fileOpeningService = new WindowsResourceOpeningService(processServiceMock.Object);

            fileOpeningService.Open(FileName);

            processServiceMock.Verify(m => m.Run(FileName), Times.Once());
        }

        [Fact]
        public void TestFileServiceOpeningLinuxDefault()
        {
            const string command = "xdg-open";
            var arguments = $"\"{FileName}\"";

            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run(command, arguments))
                .Verifiable();
            var environmentServiceMock = new Mock<IEnvironmentService>();
            environmentServiceMock
                .Setup(m => m.GetEnvironmentVariable("DESKTOP_SESSION"))
                .Returns("Unknown");

            var fileOpeningService = new LinuxResourceOpeningService(processServiceMock.Object, environmentServiceMock.Object);

            fileOpeningService.Open(FileName);

            processServiceMock.Verify(m => m.Run(command, arguments), Times.Once());
        }

        [Fact]
        public void TestFileServiceOpeningMacOs()
        {
            const string command = "open";
            var arguments = $"\"{FileName}\"";

            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run(command, arguments))
                .Verifiable();

            var fileOpeningService = new MacResourceOpeningService(processServiceMock.Object);

            fileOpeningService.Open(FileName);

            processServiceMock.Verify(m => m.Run(command, arguments), Times.Once());
        }
    }
}