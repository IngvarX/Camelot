using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Implementations.Settings;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Settings
{
    public class TerminalSettingsViewModelTests
    {
        private const string DefaultCommand = "Command";
        private const string NewCommand = "NewCommand";
        private const string DefaultArguments = "Arguments";
        private const string NewArguments = "NewArguments";

        [Fact]
        public void TestActivation()
        {
            var terminalServiceMock = new Mock<ITerminalService>();
            terminalServiceMock
                .Setup(m => m.GetTerminalSettings())
                .Returns(new TerminalSettingsStateModel
                {
                    Command = DefaultCommand,
                    Arguments = DefaultArguments
                });

            var viewModel = new TerminalSettingsViewModel(terminalServiceMock.Object);
            viewModel.Activate();

            Assert.False(viewModel.IsChanged);
            Assert.Equal(DefaultCommand, viewModel.TerminalCommandText);
            Assert.Equal(DefaultArguments, viewModel.TerminalCommandArguments);
        }

        [Fact]
        public void TestDoubleActivation()
        {
            var terminalServiceMock = new Mock<ITerminalService>();
            terminalServiceMock
                .Setup(m => m.GetTerminalSettings())
                .Returns(new TerminalSettingsStateModel())
                .Verifiable();

            var viewModel = new TerminalSettingsViewModel(terminalServiceMock.Object);
            viewModel.Activate();
            viewModel.Activate();

            terminalServiceMock.Verify(m => m.GetTerminalSettings(), Times.Once);
        }

        [Fact]
        public void TestSaveChanges()
        {
            var terminalServiceMock = new Mock<ITerminalService>();
            terminalServiceMock
                .Setup(m => m.GetTerminalSettings())
                .Returns(new TerminalSettingsStateModel
                {
                    Command = DefaultCommand,
                    Arguments = DefaultArguments
                });
            terminalServiceMock
                .Setup(m => m.SetTerminalSettings(It.Is<TerminalSettingsStateModel>(ts =>
                    ts.Command == NewCommand && ts.Arguments == NewArguments)))
                .Verifiable();

            var viewModel = new TerminalSettingsViewModel(terminalServiceMock.Object);
            viewModel.Activate();
            viewModel.TerminalCommandText = NewCommand;
            viewModel.TerminalCommandArguments = NewArguments;

            Assert.True(viewModel.IsChanged);
            viewModel.SaveChanges();

            terminalServiceMock
                .Verify(m => m.SetTerminalSettings(It.Is<TerminalSettingsStateModel>(ts =>
                    ts.Command == NewCommand && ts.Arguments == NewArguments)), Times.Once);
        }
    }
}