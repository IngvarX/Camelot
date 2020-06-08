using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class OverwriteOptionsDialogViewModelTests
    {
        private const string SourceFilePath = "Source";
        private const string DestinationFilePath = "Destination";

        [Fact]
        public void TestCanceled()
        {
            var callbackCalled = false;
            var dialog = Create();
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                if (result is null)
                {
                    callbackCalled = true;
                }
            };

            dialog.CancelCommand.Execute(null);

            Assert.True(callbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestSkip(bool shouldApplyForAll)
        {
            var callbackCalled = false;
            var dialog = Create();
            dialog.ShouldApplyForAll = shouldApplyForAll;
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                Assert.NotNull(result);
                if (result.Options.Mode is OperationContinuationMode.Skip)
                {
                    callbackCalled = true;
                }

                Assert.Equal(shouldApplyForAll, result.Options.ApplyForAll);
            };

            dialog.SkipCommand.Execute(null);

            Assert.True(callbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestOverwrite(bool shouldApplyForAll)
        {
            var callbackCalled = false;
            var dialog = Create();
            dialog.ShouldApplyForAll = shouldApplyForAll;
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                Assert.NotNull(result);
                if (result.Options.Mode is OperationContinuationMode.Overwrite)
                {
                    callbackCalled = true;
                }

                Assert.Equal(shouldApplyForAll, result.Options.ApplyForAll);
            };

            dialog.ReplaceCommand.Execute(null);

            Assert.True(callbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestOverwriteIfOlder(bool shouldApplyForAll)
        {
            var callbackCalled = false;
            var dialog = Create();
            dialog.ShouldApplyForAll = shouldApplyForAll;
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                Assert.NotNull(result);
                if (result.Options.Mode is OperationContinuationMode.OverwriteOlder)
                {
                    callbackCalled = true;
                }

                Assert.Equal(shouldApplyForAll, result.Options.ApplyForAll);
            };

            dialog.ReplaceIfOlderCommand.Execute(null);

            Assert.True(callbackCalled);
        }

        private static OverwriteOptionsDialogViewModel Create()
        {
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.GetFile(It.IsAny<string>()))
                .Returns<string>(s => new FileModel {FullPath = s, Name = s});
            var fileSystemNodeViewModelFactory = new Mock<IFileSystemNodeViewModelFactory>();
            fileSystemNodeViewModelFactory
                .Setup(m => m.Create(It.IsAny<FileModel>()))
                .Returns(new Mock<IFileSystemNodeViewModel>().Object);
            var fileNameGenerationService = new Mock<IFileNameGenerationService>();
            var pathService = new Mock<IPathService>();
            var dialog = new OverwriteOptionsDialogViewModel(
                fileServiceMock.Object, fileSystemNodeViewModelFactory.Object,
                fileNameGenerationService.Object, pathService.Object);
            var parameter = new OverwriteOptionsNavigationParameter(SourceFilePath, DestinationFilePath, true);
            dialog.Activate(parameter);

            return dialog;
        }
    }
}