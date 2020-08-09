using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class RemoveNodesConfirmationDialogViewModelTests
    {
        private const string FilePath = "FilePath";
        private const string FileName = "FileName";

        [Fact]
        public void TestCancelCommand()
        {
            var pathServiceMock = new Mock<IPathService>();
            var dialog = new RemoveNodesConfirmationDialogViewModel(pathServiceMock.Object);
            var isCallbackCalled = false;
            dialog.CloseRequested += (sender, args) => isCallbackCalled = !args.Result.IsConfirmed;

            Assert.True(dialog.CancelCommand.CanExecute(null));

            dialog.CancelCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        [Fact]
        public void TestOkCommand()
        {
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetFileName(FilePath))
                .Returns(FileName);
            var dialog = new RemoveNodesConfirmationDialogViewModel(pathServiceMock.Object);
            var isCallbackCalled = false;
            dialog.CloseRequested += (sender, args) => isCallbackCalled = args.Result.IsConfirmed;

            Assert.True(dialog.OkCommand.CanExecute(null));
            dialog.OkCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestProperties(bool isRemovingToTrash)
        {
            var pathServiceMock = new Mock<IPathService>();
            pathServiceMock
                .Setup(m => m.GetFileName(FilePath))
                .Returns(FileName);
            var dialog = new RemoveNodesConfirmationDialogViewModel(pathServiceMock.Object);

            var files = new[] {FilePath};
            dialog.Activate(new NodesRemovingNavigationParameter(files, isRemovingToTrash));

            Assert.Equal(new[] {FileName}, dialog.Files.ToArray());
            Assert.Equal(1, dialog.FilesCount);
            Assert.Equal(isRemovingToTrash, dialog.IsRemovingToTrash);
            Assert.True(dialog.ShouldShowFilesList);
        }
    }
}