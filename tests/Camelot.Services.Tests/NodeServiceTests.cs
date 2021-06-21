using Camelot.Services.Abstractions;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests
{
    public class NodeServiceTests
    {
        private const string Node = "Node";

        private readonly AutoMocker _autoMocker;

        public NodeServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(true, true, true)]
        public void TestCheckIfExists(bool fileExists, bool dirExists, bool expected)
        {
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(Node))
                .Returns(fileExists);
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(Node))
                .Returns(dirExists);

            var service = _autoMocker.CreateInstance<NodeService>();
            var actual = service.CheckIfExists(Node);

            Assert.Equal(expected, actual);
        }
    }
}