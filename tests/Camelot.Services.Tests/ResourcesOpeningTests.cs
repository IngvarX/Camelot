using System.IO;
using Camelot.Services.Abstractions;
using Camelot.Services.Behaviors;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class ResourcesOpeningTests
    {
        private const string FileName = "File.txt";
        private const string Command = "Command";
        private const string Arguments = "Args";

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

            directoryServiceMock
                .VerifySet(c => c.SelectedDirectory = CurrentDirectory, Times.Once);
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

            fileOpeningServiceMock
                .Verify(m => m.Open(FileName), Times.Once);
        }
        
        [Fact]
        public void TestFileOpeningWith()
        {
            var fileOpeningServiceMock = new Mock<IResourceOpeningService>();
            fileOpeningServiceMock
                .Setup(m => m.OpenWith(Command, Arguments, FileName))
                .Verifiable();

            var fileOpeningBehavior = new FileOpeningBehavior(fileOpeningServiceMock.Object);
            fileOpeningBehavior.OpenWith(Command, Arguments, FileName);

            fileOpeningServiceMock
                .Verify(m => m.OpenWith(Command, Arguments, FileName),
                    Times.Once);
        }
    }
}