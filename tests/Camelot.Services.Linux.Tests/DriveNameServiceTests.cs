using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Implementations;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Linux.Tests;

public class DriveNameServiceTests
{
    private readonly AutoMocker _autoMocker;

    public DriveNameServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData("/media/camelot/test", "/dev/sdc1 /media/camelot/test uid=1234,utf8 0 0\n\n\n", "/dev/sdc1")]
    [InlineData("/media/camelot/test2", "/dev/sdc1 /media/camelot/test1 uid=1234,utf8 0 0\n/dev/sdc2 /media/camelot/test2 uid=4321,utf8 0 0", "/dev/sdc2")]
    public async Task TestGetDriveName(string rootDirectory, string mounts, string expected)
    {
        _autoMocker
            .Setup<IEnvironmentService, string>(m => m.NewLine)
            .Returns("\n");
        _autoMocker
            .Setup<IProcessService, Task<string>>(m => m.ExecuteAndGetOutputAsync("cat", "/proc/mounts"))
            .ReturnsAsync(mounts);

        var service = _autoMocker.CreateInstance<DriveNameService>();

        var actual = await service.GetDriveNameAsync(rootDirectory);

        Assert.Equal(expected, actual);
    }
}