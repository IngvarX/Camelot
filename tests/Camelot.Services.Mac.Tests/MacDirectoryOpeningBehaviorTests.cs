using Camelot.Services.Abstractions.Behaviors;
using Moq;
using Xunit;

namespace Camelot.Services.Mac.Tests
{
    public class MacDirectoryOpeningBehaviorTests
    {
        [Theory]
        [InlineData("Camelot", 0, 1)]
        [InlineData("Camelot.app", 1, 0)]
        public void Test(string directoryName, int fileOpeningCalled, int directoryOpeningCalled)
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
    }
}