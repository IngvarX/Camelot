using Camelot.Services.Environment.Interfaces;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.AllPlatforms.Tests;

public class UnixHomeDirectoryProviderTests
{
    private const string HomeDirPath = "/home/camelot";

    private readonly AutoMocker _autoMocker;

    public UnixHomeDirectoryProviderTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestHomeDirectory()
    {
        _autoMocker
            .Setup<IEnvironmentService, string>(m => m.GetEnvironmentVariable("HOME"))
            .Returns(HomeDirPath);

        var homeDirProvider = _autoMocker.CreateInstance<UnixHomeDirectoryProvider>();
        var homeDir = homeDirProvider.HomeDirectoryPath;

        Assert.Equal(HomeDirPath, homeDir);
    }
}