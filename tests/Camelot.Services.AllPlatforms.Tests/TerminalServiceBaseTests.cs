using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Environment.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.AllPlatforms.Tests
{
    public class TerminalServiceBaseTests
    {
        private const string Command = "terminal";
        private const string Arguments = "arguments";

        [Fact]
        public void TestSave()
        {
            var terminalSettings = new TerminalSettings
            {
                Command = Command,
                Arguments = Arguments
            };
            var repositoryMock = new Mock<IRepository<TerminalSettings>>();
            repositoryMock
                .Setup(m => m.Upsert(It.IsAny<string>(), terminalSettings))
                .Verifiable();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(m => m.GetRepository<TerminalSettings>())
                .Returns(repositoryMock.Object);
            var uowFactoryMock = new Mock<IUnitOfWorkFactory>();
            uowFactoryMock
                .Setup(m => m.Create())
                .Returns(uowMock.Object);
            var processServiceMock = new Mock<IProcessService>();

            var terminalService = new TerminalService(processServiceMock.Object, uowFactoryMock.Object);

            terminalService.SetTerminalSettings(terminalSettings);

            repositoryMock.Verify(m => m.Upsert(It.IsAny<string>(), terminalSettings));
        }

        [Fact]
        public void TestGetSaved()
        {
            var savedTerminalSettings = new TerminalSettings
            {
                Command = "SavedCommand",
                Arguments = "SavedArguments"
            };
            var repositoryMock = new Mock<IRepository<TerminalSettings>>();
            repositoryMock
                .Setup(m => m.GetById(It.IsAny<string>()))
                .Returns(savedTerminalSettings);
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(m => m.GetRepository<TerminalSettings>())
                .Returns(repositoryMock.Object);
            var uowFactoryMock = new Mock<IUnitOfWorkFactory>();
            uowFactoryMock
                .Setup(m => m.Create())
                .Returns(uowMock.Object);
            var processServiceMock = new Mock<IProcessService>();

            var terminalService = new TerminalService(processServiceMock.Object, uowFactoryMock.Object);

            var terminalSettings = terminalService.GetTerminalSettings();
            Assert.Equal(savedTerminalSettings, terminalSettings);
        }

        [Fact]
        public void TestGetDefault()
        {
            var repositoryMock = new Mock<IRepository<TerminalSettings>>();
            var uowMock = new Mock<IUnitOfWork>();
            uowMock
                .Setup(m => m.GetRepository<TerminalSettings>())
                .Returns(repositoryMock.Object);
            var uowFactoryMock = new Mock<IUnitOfWorkFactory>();
            uowFactoryMock
                .Setup(m => m.Create())
                .Returns(uowMock.Object);
            var processServiceMock = new Mock<IProcessService>();

            var terminalService = new TerminalService(processServiceMock.Object, uowFactoryMock.Object);

            var terminalSettings = terminalService.GetTerminalSettings();

            Assert.NotNull(terminalSettings);
            Assert.Equal(Command, terminalSettings.Command);
            Assert.Equal(Arguments, terminalSettings.Arguments);
        }

        private class TerminalService : TerminalServiceBase
        {
            public TerminalService(IProcessService processService, IUnitOfWorkFactory unitOfWorkFactory)
                : base(processService, unitOfWorkFactory)
            {

            }

            protected override TerminalSettings GetDefaultSettings() => new TerminalSettings
            {
                Command = Command,
                Arguments = Arguments
            };
        }
    }
}