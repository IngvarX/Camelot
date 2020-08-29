using Camelot.Services.Environment.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsHomeDirectoryProviderTests
    {
        private const string HomeDirPath = "C:/camelot";

        [Fact]
        public void TestHomeDirectory()
        {
            var envServiceMock = new Mock<IEnvironmentService>();
            envServiceMock
                .Setup(m => m.GetEnvironmentVariable("USERPROFILE"))
                .Returns(HomeDirPath);

            var homeDirProvider = new WindowsHomeDirectoryProvider(envServiceMock.Object);
            var homeDir = homeDirProvider.HomeDirectoryPath;

            Assert.Equal(HomeDirPath, homeDir);
        }
    }
}