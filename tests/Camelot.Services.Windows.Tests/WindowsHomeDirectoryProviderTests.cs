using Camelot.Services.Environment.Interfaces;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Windows.Tests;

public class WindowsHomeDirectoryProviderTests
{
    private const string HomeDirPath = "C:/camelot";

    private readonly AutoMocker _autoMocker;

    public WindowsHomeDirectoryProviderTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestHomeDirectory()
    {
        _autoMocker
            .Setup<IEnvironmentService, string>(m => m.GetEnvironmentVariable("USERPROFILE"))
            .Returns(HomeDirPath);

        var homeDirProvider = _autoMocker.CreateInstance<WindowsHomeDirectoryProvider>();
        var homeDir = homeDirProvider.HomeDirectoryPath;

        Assert.Equal(HomeDirPath, homeDir);
    }
}