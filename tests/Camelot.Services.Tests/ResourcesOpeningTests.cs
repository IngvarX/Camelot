using System.IO;
using Camelot.Services.Abstractions;
using Camelot.Services.Behaviors;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests
{
    public class ResourcesOpeningTests
    {
        private const string FileName = "File.txt";
        private const string Command = "Command";
        private const string Arguments = "Args";

        private static string CurrentDirectory => Directory.GetCurrentDirectory();

        private readonly AutoMocker _autoMocker;

        public ResourcesOpeningTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestDirectoryOpening()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .SetupSet<IDirectoryService>(m => m.SelectedDirectory = CurrentDirectory)
                .Verifiable();
            _autoMocker.Use(directoryServiceMock.Object);

            var directoryOpeningBehavior = _autoMocker.CreateInstance<DirectoryOpeningBehavior>();
            directoryOpeningBehavior.Open(CurrentDirectory);

            directoryServiceMock
                .VerifySet(c => c.SelectedDirectory = CurrentDirectory, Times.Once);
        }

        [Fact]
        public void TestFileOpening()
        {
            _autoMocker
                .Setup<IResourceOpeningService>(m => m.Open(FileName))
                .Verifiable();

            var fileOpeningBehavior = _autoMocker.CreateInstance<FileOpeningBehavior>();
            fileOpeningBehavior.Open(FileName);

            _autoMocker
                .Verify<IResourceOpeningService>(m => m.Open(FileName),
                    Times.Once);
        }

        [Fact]
        public void TestFileOpeningWith()
        {
            _autoMocker
                .Setup<IResourceOpeningService>(m => m.OpenWith(Command, Arguments, FileName))
                .Verifiable();

            var fileOpeningBehavior = _autoMocker.CreateInstance<FileOpeningBehavior>();
            fileOpeningBehavior.OpenWith(Command, Arguments, FileName);

            _autoMocker
                .Verify<IResourceOpeningService>(m => m.OpenWith(Command, Arguments, FileName),
                    Times.Once);
        }

        [Fact]
        public void TestDirectoryOpeningWith()
        {
            _autoMocker
                .Setup<IResourceOpeningService>(m => m.OpenWith(Command, Arguments, FileName))
                .Verifiable();

            var directoryOpeningBehavior = _autoMocker.CreateInstance<DirectoryOpeningBehavior>();
            directoryOpeningBehavior.OpenWith(Command, Arguments, FileName);

            _autoMocker
                .Verify<IResourceOpeningService>(m => m.OpenWith(Command, Arguments, FileName),
                    Times.Once);
        }
    }
}