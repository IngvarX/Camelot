using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Environment.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Mac.Tests;

public class MacTerminalServiceTests
{
    private const string Directory = "Dir";

    private readonly AutoMocker _autoMocker;

    public MacTerminalServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestOpening()
    {
        const string command = "open";
        var args = $"-a Terminal \"{Directory}\"";

        var uowMock = new Mock<IUnitOfWork>();
        uowMock
            .Setup(m => m.GetRepository<TerminalSettings>())
            .Returns(new Mock<IRepository<TerminalSettings>>().Object);
        _autoMocker
            .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
            .Returns(uowMock.Object);
        _autoMocker
            .Setup<IProcessService>(m => m.Run(command, args))
            .Verifiable();

        var terminalService = _autoMocker.CreateInstance<MacTerminalService>();

        terminalService.Open(Directory);

        _autoMocker.Verify<IProcessService>(m => m.Run(command, args), Times.Once);
    }
}