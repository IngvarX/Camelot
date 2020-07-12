using Camelot.Avalonia.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Menu;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class MenuViewModelTests
    {
        [Fact]
        public void TestAppClosing()
        {
            var applicationCloserMock = new Mock<IApplicationCloser>();
            applicationCloserMock
                .Setup(m => m.CloseApp())
                .Verifiable();
            var dialogServiceMock = new Mock<IDialogService>();
            var menuViewModel = new MenuViewModel(applicationCloserMock.Object, dialogServiceMock.Object);

            Assert.True(menuViewModel.ExitCommand.CanExecute(null));
            menuViewModel.ExitCommand.Execute(null);

            applicationCloserMock.Verify(m => m.CloseApp(), Times.Once());
        }

        [Fact]
        public void TestAboutDialogOpening()
        {
            var applicationCloserMock = new Mock<IApplicationCloser>();
            var dialogServiceMock = new Mock<IDialogService>();
            dialogServiceMock
                .Setup(m => m.ShowDialogAsync(nameof(AboutDialogViewModel)))
                .Verifiable();
            var menuViewModel = new MenuViewModel(applicationCloserMock.Object, dialogServiceMock.Object);

            Assert.True(menuViewModel.AboutCommand.CanExecute(null));
            menuViewModel.AboutCommand.Execute(null);

            dialogServiceMock.Verify(m => m.ShowDialogAsync(nameof(AboutDialogViewModel)), Times.Once());
        }

        [Fact]
        public void TestSettingsDialogOpening()
        {
            var applicationCloserMock = new Mock<IApplicationCloser>();
            var dialogServiceMock = new Mock<IDialogService>();
            dialogServiceMock
                .Setup(m => m.ShowDialogAsync(nameof(SettingsDialogViewModel)))
                .Verifiable();
            var menuViewModel = new MenuViewModel(applicationCloserMock.Object, dialogServiceMock.Object);

            Assert.True(menuViewModel.OpenSettingsCommand.CanExecute(null));
            menuViewModel.OpenSettingsCommand.Execute(null);

            dialogServiceMock.Verify(m => m.ShowDialogAsync(nameof(SettingsDialogViewModel)), Times.Once());
        }
    }
}