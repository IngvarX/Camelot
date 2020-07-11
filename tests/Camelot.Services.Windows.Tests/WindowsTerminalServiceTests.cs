using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Environment.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsTerminalServiceTests
    {
        private const string Directory = "Dir";

        [Fact]
        public void TestOpening()
        {
            const string command = "cmd";
            var args = $"/K \"cd /d {Directory}\"";

            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(m => m.GetRepository<TerminalSettings>())
                .Returns(new Mock<IRepository<TerminalSettings>>().Object);
            var uowFactoryMock = new Mock<IUnitOfWorkFactory>();
            uowFactoryMock
                .Setup(m => m.Create())
                .Returns(uowMock.Object);
            var processServiceMock = new Mock<IProcessService>();
            processServiceMock
                .Setup(m => m.Run(command, args))
                .Verifiable();

            var terminalService = new WindowsTerminalService(processServiceMock.Object,
                uowFactoryMock.Object);

            terminalService.Open(Directory);

            processServiceMock.Verify(m => m.Run(command, args), Times.Once);
        }
    }
}