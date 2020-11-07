using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class CreateFileDialogViewModelTests
    {
        private const string DirectoryPath = "Directory";
        private const string FileName = "Name";
        private const string NewFilePath = "Directory/Name";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void TestFileWithWhiteSpaceCreation(string fileName)
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileServiceMock = new Mock<IFileService>();
            var pathServiceMock = new Mock<IPathService>();

            var dialog = new CreateFileDialogViewModel(
                directoryServiceMock.Object, fileServiceMock.Object, pathServiceMock.Object);
            dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

            Assert.False(dialog.CreateCommand.CanExecute(null));

            dialog.FileName = fileName;

            Assert.False(dialog.CreateCommand.CanExecute(null));
        }

        [Fact]
        public void TestDirectoryWithExistingDirectoryNameCreation()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.CheckIfExists(NewFilePath))
                .Returns(true);
            var fileServiceMock = new Mock<IFileService>();
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.Combine(DirectoryPath, FileName))
                .Returns(NewFilePath);

            var dialog = new CreateFileDialogViewModel(
                directoryServiceMock.Object, fileServiceMock.Object, pathServiceMock.Object);
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
            var directoryServiceMock = new Mock<IDirectoryService>();
            directoryServiceMock
                .Setup(m => m.CheckIfExists(NewFilePath))
                .Returns(dirExists);
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.CheckIfExists(NewFilePath))
                .Returns(fileExists);
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.Combine(DirectoryPath, FileName))
                .Returns(NewFilePath);

            var dialog = new CreateFileDialogViewModel(
                directoryServiceMock.Object, fileServiceMock.Object, pathServiceMock.Object);
            dialog.Activate(new CreateNodeNavigationParameter(DirectoryPath));

            dialog.FileName = FileName;

            Assert.False(dialog.CreateCommand.CanExecute(null));
        }

        [Fact]
        public void TestDirectoryCreation()
        {
            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileServiceMock = new Mock<IFileService>();
            var pathServiceMock = new Mock<IPathService>();

            var dialog = new CreateFileDialogViewModel(
                directoryServiceMock.Object, fileServiceMock.Object, pathServiceMock.Object);
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
            var directoryServiceMock = new Mock<IDirectoryService>();
            var fileServiceMock = new Mock<IFileService>();
            var pathServiceMock = new Mock<IPathService>();

            var dialog = new CreateFileDialogViewModel(
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