using Camelot.Services.Environment.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.AllPlatforms.Tests
{
    public class UnixHomeDirectoryProviderTests
    {
        private const string HomeDirPath = "/home/camelot";

        [Fact]
        public void TestHomeDirectory()
        {
            var envServiceMock = new Mock<IEnvironmentService>();
            envServiceMock
                .Setup(m => m.GetEnvironmentVariable("HOME"))
                .Returns(HomeDirPath);

            var homeDirProvider = new UnixHomeDirectoryProvider(envServiceMock.Object);
            var homeDir = homeDirProvider.HomeDirectoryPath;

            Assert.Equal(HomeDirPath, homeDir);
        }
    }
}