using System.IO;
using Camelot.Services.Abstractions;
using Camelot.Services.Behaviors;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Interfaces;
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
            var platformServiceMock = new Mock<IPlatformService>();
            platformServiceMock
                .Setup(m => m.GetPlatform())
                .Returns(Platform.Windows);

            var fileOpeningService = new ResourceOpeningService(
                processServiceMock.Object, platformServiceMock.Object);

            fileOpeningService.Open(FileName);

            processServiceMock.Verify(m => m.Run(FileName), Times.Once());
        }

        [Fact]
        public void TestFileServiceOpeningLinux()
        {
            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run("xdg-open", $"\"{FileName}\""))
                .Verifiable();
            var platformServiceMock = new Mock<IPlatformService>();
            platformServiceMock
                .Setup(m => m.GetPlatform())
                .Returns(Platform.Linux);

            var fileOpeningService = new ResourceOpeningService(
                processServiceMock.Object, platformServiceMock.Object);

            fileOpeningService.Open(FileName);

            processServiceMock.Verify(m => m.Run("xdg-open", $"\"{FileName}\""), Times.Once());
        }

        [Fact]
        public void TestFileServiceOpeningMacOs()
        {
            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run("open", $"\"{FileName}\""))
                .Verifiable();
            var platformServiceMock = new Mock<IPlatformService>();
            platformServiceMock
                .Setup(m => m.GetPlatform())
                .Returns(Platform.MacOs);

            var fileOpeningService = new ResourceOpeningService(
                processServiceMock.Object, platformServiceMock.Object);

            fileOpeningService.Open(FileName);

            processServiceMock.Verify(m => m.Run("open", $"\"{FileName}\""), Times.Once());
        }
    }
}