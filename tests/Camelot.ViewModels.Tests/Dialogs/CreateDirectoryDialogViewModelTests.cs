using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class CreateDirectoryDialogViewModelTests
    {
        private const string DirectoryPath = "Directory";
        private const string DirectoryName = "Name";
        private const string NewDirectoryPath = "Directory/Name";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void TestDirectoryWithWhiteSpaceCreation(string directoryName)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileServiceMock = new Mock<IFileService>();
            var pathServiceMock = new Mock<IPathService>();

            var dialog = new CreateDirectoryDialogViewModel(
                directoryServiceMock.Object, fileServiceMock.Object, pathServiceMock.Object);
            dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

            Assert.False(dialog.CreateCommand.CanExecute(null));

            dialog.DirectoryName = directoryName;

            Assert.False(dialog.CreateCommand.CanExecute(null));
        }

        [Fact]
        public void TestDirectoryWithExistingDirectoryNameCreation()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.CheckIfExists(NewDirectoryPath))
                .Returns(true);
            var fileServiceMock = new Mock<IFileService>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.Combine(DirectoryPath, DirectoryName))
                .Returns(NewDirectoryPath);

            var dialog = new CreateDirectoryDialogViewModel(
                directoryServiceMock.Object, fileServiceMock.Object, pathServiceMock.Object);
            dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

            dialog.DirectoryName = DirectoryName;

            Assert.False(dialog.CreateCommand.CanExecute(null));
        }

        [Fact]
        public void TestDirectoryWithExistingFileNameCreation()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfExists(NewDirectoryPath))
                .Returns(true);
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.Combine(DirectoryPath, DirectoryName))
                .Returns(NewDirectoryPath);

            var dialog = new CreateDirectoryDialogViewModel(
                directoryServiceMock.Object, fileServiceMock.Object, pathServiceMock.Object);
            dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

            dialog.DirectoryName = DirectoryName;

            Assert.False(dialog.CreateCommand.CanExecute(null));
        }

        [Fact]
        public void TestDirectoryCreation()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileServiceMock = new Mock<IFileService>();
            var pathServiceMock = new Mock<IPathService>();

            var dialog = new CreateDirectoryDialogViewModel(
                directoryServiceMock.Object, fileServiceMock.Object, pathServiceMock.Object);
            dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

            var isCallbackCalled = false;
            dialog.CloseRequested += (sender, args) =>
            {
                if (args.Result.DirectoryName == DirectoryName)
                {
                    isCallbackCalled = true;
                }
            };

            dialog.DirectoryName = DirectoryName;

            Assert.Equal(DirectoryName, dialog.DirectoryName);
            Assert.True(dialog.CreateCommand.CanExecute(null));

            dialog.CreateCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        [Fact]
        public void TestCancel()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileServiceMock = new Mock<IFileService>();
            var pathServiceMock = new Mock<IPathService>();

            var dialog = new CreateDirectoryDialogViewModel(
                directoryServiceMock.Object, fileServiceMock.Object, pathServiceMock.Object);
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