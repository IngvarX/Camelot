using Camelot.Services.Abstractions.Behaviors;
using Moq;
using Xunit;

namespace Camelot.Services.Mac.Tests
{
    public class MacDirectoryOpeningBehaviorTests
    {
        private const string Command = "Command";
        private const string Arguments = "Args";
        
        [Theory]
        [InlineData("Camelot", 0, 1)]
        [InlineData("Camelot.app", 1, 0)]
        public void TestOpen(string directoryName, int fileOpeningCalled, int directoryOpeningCalled)
        {
            var fileOpeningBehaviorMock = new Mock<IFileSystemNodeOpeningBehavior>();
            fileOpeningBehaviorMock
                .Setup(m => m.Open(directoryName))
                .Verifiable();
            var directoryOpeningBehaviorMock = new Mock<IFileSystemNodeOpeningBehavior>();
            directoryOpeningBehaviorMock
                .Setup(m => m.Open(directoryName))
                .Verifiable();

            var behavior = new MacDirectoryOpeningBehavior(
                fileOpeningBehaviorMock.Object, directoryOpeningBehaviorMock.Object);

            behavior.Open(directoryName);

            fileOpeningBehaviorMock
                .Verify(m => m.Open(directoryName),
                    Times.Exactly(fileOpeningCalled));
            directoryOpeningBehaviorMock
                .Verify(m => m.Open(directoryName),
                    Times.Exactly(directoryOpeningCalled));
        }
        
        [Theory]
        [InlineData("Camelot", 0, 1)]
        [InlineData("Camelot.app", 1, 0)]
        public void TestOpenWith(string directoryName, int fileOpeningCalled, int directoryOpeningCalled)
        {
            var fileOpeningBehaviorMock = new Mock<IFileSystemNodeOpeningBehavior>();
            fileOpeningBehaviorMock
                .Setup(m => m.OpenWith(Command, Arguments, directoryName))
                .Verifiable();
            var directoryOpeningBehaviorMock = new Mock<IFileSystemNodeOpeningBehavior>();
            directoryOpeningBehaviorMock
                .Setup(m => m.OpenWith(Command, Arguments, directoryName))
                .Verifiable();

            var behavior = new MacDirectoryOpeningBehavior(
                fileOpeningBehaviorMock.Object, directoryOpeningBehaviorMock.Object);

            behavior.OpenWith(Command, Arguments, directoryName);

            fileOpeningBehaviorMock
                .Verify(m => m.OpenWith(Command, Arguments, directoryName),
                    Times.Exactly(fileOpeningCalled));
            directoryOpeningBehaviorMock
                .Verify(m => m.OpenWith(Command, Arguments, directoryName),
                    Times.Exactly(directoryOpeningCalled));
        }
    }
}