using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class CreateFileDialogViewModelTests
    {
        private const string DirectoryPath = "Directory";
        private const string FileName = "Name";
        private const string NewFilePath = "Directory/Name";

        private readonly AutoMocker _autoMocker;

        public CreateFileDialogViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void TestFileWithWhiteSpaceCreation(string fileName)
        {
            var dialog = _autoMocker.CreateInstance<CreateFileDialogViewModel>();
            dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

            Assert.False(dialog.CreateCommand.CanExecute(null));

            dialog.FileName = fileName;

            Assert.False(dialog.CreateCommand.CanExecute(null));
        }

        [Fact]
        public void TestDirectoryWithExistingDirectoryNameCreation()
        {
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(NewFilePath))
                .Returns(true);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(DirectoryPath, FileName))
                .Returns(NewFilePath);

            var dialog = _autoMocker.CreateInstance<CreateFileDialogViewModel>();
            dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

            dialog.FileName = FileName;

            Assert.False(dialog.CreateCommand.CanExecute(null));
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void TestFileWithExistingFileNameCreation(bool fileExists, bool dirExists)
        {
            _autoMocker
                .Setup<IDirectoryService, bool>(m => m.CheckIfExists(NewFilePath))
                .Returns(dirExists);
            _autoMocker
                .Setup<IFileService, bool>(m => m.CheckIfExists(NewFilePath))
                .Returns(fileExists);
            _autoMocker
                .Setup<IPathService, string>(m => m.Combine(DirectoryPath, FileName))
                .Returns(NewFilePath);

            var dialog = _autoMocker.CreateInstance<CreateFileDialogViewModel>();
            dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

            dialog.FileName = FileName;

            Assert.False(dialog.CreateCommand.CanExecute(null));
        }

        [Fact]
        public void TestDirectoryCreation()
        {
            var dialog = _autoMocker.CreateInstance<CreateFileDialogViewModel>();
            dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

            var isCallbackCalled = false;
            dialog.CloseRequested += (sender, args) =>
            {
                if (args.Result.FileName == FileName)
                {
                    isCallbackCalled = true;
                }
            };

            dialog.FileName = FileName;

            Assert.Equal(FileName, dialog.FileName);
            Assert.True(dialog.CreateCommand.CanExecute(null));

            dialog.CreateCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        [Fact]
        public void TestCancel()
        {
            var dialog = _autoMocker.CreateInstance<CreateFileDialogViewModel>();
            dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

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