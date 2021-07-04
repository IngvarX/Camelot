using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Interfaces.Settings;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class SettingsDialogViewModelTests
    {
        [Fact]
        public void TestSettingsViewModel()
        {
            var generalSettingsViewModel = new Mock<ISettingsViewModel>();
            generalSettingsViewModel
                .Setup(m => m.Activate())
                .Verifiable();

            var terminalSettingsViewModel = new Mock<ISettingsViewModel>();
            terminalSettingsViewModel
                .Setup(m => m.Activate())
                .Verifiable();

            var dialogViewModel = new SettingsDialogViewModel(generalSettingsViewModel.Object, terminalSettingsViewModel.Object);

            Assert.Equal(0, dialogViewModel.SelectedIndex);
            Assert.Equal(generalSettingsViewModel.Object, dialogViewModel.GeneralSettingsViewModel);
            Assert.Equal(terminalSettingsViewModel.Object, dialogViewModel.TerminalSettingsViewModel);

            generalSettingsViewModel.Verify(m => m.Activate(), Times.Once);
        }

        [Fact]
        public void TestSaveCommand()
        {
            var generalSettingsViewModel = new Mock<ISettingsViewModel>();
            generalSettingsViewModel
                .Setup(m => m.Activate())
                .Verifiable();
            generalSettingsViewModel
                .SetupGet(m => m.IsChanged)
                .Returns(true);

            var terminalSettingsViewModel = new Mock<ISettingsViewModel>();
            terminalSettingsViewModel
                .Setup(m => m.SaveChanges())
                .Verifiable();
            terminalSettingsViewModel
                .SetupGet(m => m.IsChanged)
                .Returns(true);

            var dialogViewModel = new SettingsDialogViewModel(generalSettingsViewModel.Object, terminalSettingsViewModel.Object);

            Assert.True(dialogViewModel.SaveCommand.CanExecute(null));
            dialogViewModel.SaveCommand.Execute(null);

            generalSettingsViewModel.Verify(m => m.SaveChanges(), Times.Once);
            terminalSettingsViewModel.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void TestSaveCommandNoChanges()
        {
            var generalSettingsViewModel = new Mock<ISettingsViewModel>();
            generalSettingsViewModel
                .Setup(m => m.Activate())
                .Verifiable();

            var terminalSettingsViewModel = new Mock<ISettingsViewModel>();
            terminalSettingsViewModel
                .Setup(m => m.Activate())
                .Verifiable();

            var dialogViewModel = new SettingsDialogViewModel(generalSettingsViewModel.Object, terminalSettingsViewModel.Object);

            Assert.True(dialogViewModel.SaveCommand.CanExecute(null));
            dialogViewModel.SaveCommand.Execute(null);

            generalSettingsViewModel.Verify(m => m.SaveChanges(), Times.Never);
            terminalSettingsViewModel.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Fact]
        public void TestSettingsViewModelActivation()
        {
            var generalSettingsViewModel = new Mock<ISettingsViewModel>();
            generalSettingsViewModel
                .Setup(m => m.Activate())
                .Verifiable();

            var terminalSettingsViewModel = new Mock<ISettingsViewModel>();
            terminalSettingsViewModel
                .Setup(m => m.Activate())
                .Verifiable();

            var dialogViewModel = new SettingsDialogViewModel(generalSettingsViewModel.Object, terminalSettingsViewModel.Object)
            {
                SelectedIndex = 0
            };
            Assert.Equal(0, dialogViewModel.SelectedIndex);

            generalSettingsViewModel.Verify(m => m.Activate(), Times.Exactly(2));
            terminalSettingsViewModel.Verify(m => m.Activate(), Times.Never);
        }
    }
}