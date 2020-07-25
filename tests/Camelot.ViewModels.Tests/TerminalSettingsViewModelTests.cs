using Camelot.DataAccess.Models;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Settings;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
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
                .Returns(new TerminalSettings
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
                .Returns(new TerminalSettings())
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
                .Returns(new TerminalSettings
                {
                    Command = DefaultCommand,
                    Arguments = DefaultArguments
                });
            var isCallbackCalled = false;
            terminalServiceMock
                .Setup(m => m.SetTerminalSettings(It.IsAny<TerminalSettings>()))
                .Callback<TerminalSettings>(ts =>
                    isCallbackCalled = ts.Command == NewCommand && ts.Arguments == NewArguments);

            var viewModel = new TerminalSettingsViewModel(terminalServiceMock.Object);
            viewModel.Activate();
            viewModel.TerminalCommandText = NewCommand;
            viewModel.TerminalCommandArguments = NewArguments;

            Assert.True(viewModel.IsChanged);
            viewModel.SaveChanges();

            Assert.True(isCallbackCalled);
        }
    }
}