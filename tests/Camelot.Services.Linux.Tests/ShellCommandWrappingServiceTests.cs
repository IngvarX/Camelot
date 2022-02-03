using Camelot.Services.Linux.Implementations;
using Xunit;

namespace Camelot.Services.Linux.Tests;

public class ShellCommandWrappingServiceTests
{
    private const string Command = "command";
    private const string Arguments = "arguments";

    [Fact]
    public void TestWrapWithNohup()
    {
        var shellCommandWrappingService = new ShellCommandWrappingService();
        var (command, arguments) = shellCommandWrappingService.WrapWithNohup(Command, Arguments);

        Assert.Equal("bash",command);
        Assert.Contains(Command, arguments);
        Assert.Contains(Arguments, arguments);
        Assert.Contains("nohup", arguments);
    }
}