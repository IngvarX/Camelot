using System.IO;
using Camelot.Services.Behaviors.Implementations;
using Camelot.Services.Enums;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class FileOpeningTests
    {
        private const string File = "File.txt";

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

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
            var fileOpeningServiceMock = new Mock<IFileOpeningService>();
            fileOpeningServiceMock
                .Setup(m => m.Open(File))
                .Verifiable();

            var fileOpeningBehavior = new FileOpeningBehavior(fileOpeningServiceMock.Object);
            fileOpeningBehavior.Open(File);

            fileOpeningServiceMock.Verify(m => m.Open(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void TestFileServiceOpeningWindows()
        {
            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run(File))
                .Verifiable();
            var platformServiceMock = new Mock<IPlatformService>();
            platformServiceMock
                .Setup(m => m.GetPlatform())
                .Returns(Platform.Windows);

            var fileOpeningService = new FileOpeningService(
                processServiceMock.Object, platformServiceMock.Object);

            fileOpeningService.Open(File);

            processServiceMock.Verify(m => m.Run(File), Times.Once());
        }

        [Fact]
        public void TestFileServiceOpeningLinux()
        {
            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run("xdg-open", $"\"{File}\""))
                .Verifiable();
            var platformServiceMock = new Mock<IPlatformService>();
            platformServiceMock
                .Setup(m => m.GetPlatform())
                .Returns(Platform.Linux);

            var fileOpeningService = new FileOpeningService(
                processServiceMock.Object, platformServiceMock.Object);

            fileOpeningService.Open(File);

            processServiceMock.Verify(m => m.Run("xdg-open", $"\"{File}\""), Times.Once());
        }

        [Fact]
        public void TestFileServiceOpeningMacOs()
        {
            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run("open", $"\"{File}\""))
                .Verifiable();
            var platformServiceMock = new Mock<IPlatformService>();
            platformServiceMock
                .Setup(m => m.GetPlatform())
                .Returns(Platform.MacOs);

            var fileOpeningService = new FileOpeningService(
                processServiceMock.Object, platformServiceMock.Object);

            fileOpeningService.Open(File);

            processServiceMock.Verify(m => m.Run("open", $"\"{File}\""), Times.Once());
        }
    }
}