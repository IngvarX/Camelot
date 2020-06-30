using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Interfaces.Settings;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class SettingsDialogViewModelTests
    {
        [Fact]
        public void TestTerminalSettingsViewModel()
        {
            var terminalSettingsViewModel = new Mock<ISettingsViewModel>();
            terminalSettingsViewModel
                .Setup(m => m.Activate())
                .Verifiable();
            var dialogViewModel = new SettingsDialogViewModel(terminalSettingsViewModel.Object);

            Assert.Equal(0, dialogViewModel.SelectedIndex);
            Assert.Equal(terminalSettingsViewModel.Object, dialogViewModel.TerminalSettingsViewModel);

            terminalSettingsViewModel.Verify(m => m.Activate(), Times.Once);
        }

        [Fact]
        public void TestSaveCommand()
        {
            var terminalSettingsViewModel = new Mock<ISettingsViewModel>();
            terminalSettingsViewModel
                .Setup(m => m.SaveChanges())
                .Verifiable();
            terminalSettingsViewModel
                .SetupGet(m => m.IsChanged)
                .Returns(true);
            var dialogViewModel = new SettingsDialogViewModel(terminalSettingsViewModel.Object);

            Assert.True(dialogViewModel.SaveCommand.CanExecute(null));
            dialogViewModel.SaveCommand.Execute(null);

            terminalSettingsViewModel.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void TestSaveCommandNoChanges()
        {
            var terminalSettingsViewModel = new Mock<ISettingsViewModel>();
            terminalSettingsViewModel
                .Setup(m => m.SaveChanges())
                .Verifiable();
            var dialogViewModel = new SettingsDialogViewModel(terminalSettingsViewModel.Object);

            Assert.True(dialogViewModel.SaveCommand.CanExecute(null));
            dialogViewModel.SaveCommand.Execute(null);

            terminalSettingsViewModel.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Fact]
        public void TestCloseCommand()
        {
            var terminalSettingsViewModel = new Mock<ISettingsViewModel>();
            var dialogViewModel = new SettingsDialogViewModel(terminalSettingsViewModel.Object);
            var isCallbackCalled = false;
            dialogViewModel.CloseRequested += (sender, args) => isCallbackCalled = args.Result == default;
            Assert.True(dialogViewModel.CloseCommand.CanExecute(null));
            dialogViewModel.CloseCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }
    }
}