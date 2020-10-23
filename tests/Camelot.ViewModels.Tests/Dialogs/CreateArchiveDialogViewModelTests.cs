using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class CreateArchiveDialogViewModelTests
    {
        private const string ArchivePath = "Archive";
        private const string NewArchivePath = "Archive";

        private readonly AutoMocker _autoMocker;

        public CreateArchiveDialogViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void TestArchiveWithWhiteSpaceCreation(string archivePath)
        {
            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(archivePath, true));

            Assert.False(dialog.CreateCommand.CanExecute(null));

            dialog.ArchivePath = archivePath;

            Assert.False(dialog.CreateCommand.CanExecute(null));
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void TestArchiveWithExistingNodeNameCreation(bool fileExists, bool dirExists)
        {
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(NewArchivePath))
                .Returns(dirExists);
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(NewArchivePath))
                .Returns(fileExists);

            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, true));

            dialog.ArchivePath = NewArchivePath;

            Assert.False(dialog.CreateCommand.CanExecute(null));
        }

        [Fact]
        public void TestProperties()
        {
            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, true));

            Assert.Equal(ArchivePath, dialog.ArchivePath);
            Assert.Equal(ArchiveType.Zip, dialog.SelectedArchiveType.ArchiveType);
        }

        [Theory]
        [InlineData(ArchiveType.Zip)]
        [InlineData(ArchiveType.TarGz)]
        [InlineData(ArchiveType.Tar)]
        public void TestArchiveCreation(ArchiveType archiveType)
        {
            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, true));

            var isCallbackCalled = false;
            dialog.CloseRequested += (sender, args) =>
            {
                if (args.Result.ArchivePath == NewArchivePath && args.Result.ArchiveType == archiveType)
                {
                    isCallbackCalled = true;
                }
            };

            dialog.ArchivePath = NewArchivePath;

            Assert.Equal(NewArchivePath, dialog.ArchivePath);
            Assert.True(dialog.CreateCommand.CanExecute(null));

            dialog.CreateCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        [Fact]
        public void TestCancel()
        {
            var dialog = _autoMocker.CreateInstance<CreateArchiveDialogViewModel>();
            dialog.Activate(new CreateArchiveNavigationParameter(ArchivePath, true));

            var isCallbackCalled = false;
            dialog.CloseRequested += (sender, args) =>
            {
                if (args.Result is null)
                {
                    isCallbackCalled = true;
                }
            };

            Assert.True(dialog.CancelCommand.CanExecute(null));

            dialog.CancelCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }
    }
}