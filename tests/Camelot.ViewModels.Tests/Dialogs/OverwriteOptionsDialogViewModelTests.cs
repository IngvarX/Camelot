using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Dialogs
{
    public class OverwriteOptionsDialogViewModelTests
    {
        private const string SourceFilePath = "Source";
        private const string DestinationFilePath = "Destination";
        private const string NewFileName = "New";
        private const string ParentDirectory = "Parent";
        private const string ParentDirectoryName = "ParentName";
        private const string NewFilePath = ParentDirectory + NewFileName;

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void TestProperties(bool shouldApplyToAll, bool areMultipleFilesAvailable)
        {
            var dialog = Create(areMultipleFilesAvailable);
            dialog.ShouldApplyToAll = shouldApplyToAll;
            dialog.NewFileName = NewFileName;

            Assert.Equal(shouldApplyToAll, dialog.ShouldApplyToAll);
            Assert.Equal(areMultipleFilesAvailable, dialog.AreMultipleFilesAvailable);
            Assert.Equal(ParentDirectoryName, dialog.DestinationDirectoryName);
            Assert.Equal(NewFileName, dialog.NewFileName);
        }

        [Fact]
        public void TestCanceled()
        {
            var isCallbackCalled = false;
            var dialog = Create();
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                if (result is null)
                {
                    isCallbackCalled = true;
                }
            };

            dialog.CancelCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestSkip(bool shouldApplyToAll)
        {
            var isCallbackCalled = false;
            var dialog = Create();
            dialog.ShouldApplyToAll = shouldApplyToAll;
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                Assert.NotNull(result);
                if (result.Options.Mode is OperationContinuationMode.Skip)
                {
                    isCallbackCalled = true;
                }

                Assert.Equal(shouldApplyToAll, result.Options.ApplyToAll);
                Assert.Equal(SourceFilePath, result.Options.FilePath);
            };

            dialog.SkipCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestOverwrite(bool shouldApplyToAll)
        {
            var isCallbackCalled = false;
            var dialog = Create();
            dialog.ShouldApplyToAll = shouldApplyToAll;
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                Assert.NotNull(result);
                if (result.Options.Mode is OperationContinuationMode.Overwrite)
                {
                    isCallbackCalled = true;
                }

                Assert.Equal(shouldApplyToAll, result.Options.ApplyToAll);
                Assert.Equal(SourceFilePath, result.Options.FilePath);
            };

            dialog.ReplaceCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestOverwriteIfOlder(bool shouldApplyToAll)
        {
            var isCallbackCalled = false;
            var dialog = Create();
            dialog.ShouldApplyToAll = shouldApplyToAll;
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                Assert.NotNull(result);
                if (result.Options.Mode is OperationContinuationMode.OverwriteIfOlder)
                {
                    isCallbackCalled = true;
                }

                Assert.Equal(shouldApplyToAll, result.Options.ApplyToAll);
                Assert.Equal(SourceFilePath, result.Options.FilePath);
            };

            dialog.ReplaceIfOlderCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestRename(bool shouldApplyToAll)
        {
            var isCallbackCalled = false;
            var dialog = Create();
            dialog.ShouldApplyToAll = shouldApplyToAll;
            dialog.NewFileName = NewFileName;
            dialog.CloseRequested += (sender, args) =>
            {
                var result = args.Result;
                Assert.NotNull(result);
                if (result.Options.Mode is OperationContinuationMode.Rename)
                {
                    isCallbackCalled = true;
                }

                Assert.Equal(shouldApplyToAll, result.Options.ApplyToAll);
                Assert.Equal(SourceFilePath, result.Options.FilePath);
                Assert.Equal(NewFilePath, result.Options.NewFilePath);
            };

            dialog.RenameCommand.Execute(null);

            Assert.True(isCallbackCalled);
        }

        private static OverwriteOptionsDialogViewModel Create(bool areMultipleFilesAvailable = true)
        {
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(m => m.GetFile(It.IsAny<string>()))
                .Returns<string>(s => new FileModel {FullPath = s, Name = s});
            var fileSystemNodeViewModelFactory = new Mock<IFileSystemNodeViewModelFactory>();
            fileSystemNodeViewModelFactory
                .Setup(m => m.Create(It.IsAny<FileModel>()))
                .Returns<FileModel>(fm =>
                {
                    var mock = new Mock<IFileSystemNodeViewModel>();
                    mock
                        .SetupGet(m => m.FullPath)
                        .Returns(fm.FullPath);

                    return mock.Object;
                });
            var fileNameGenerationService = new Mock<IFileNameGenerationService>();
            var pathService = new Mock<IPathService>();
            pathService
                .Setup(m => m.GetParentDirectory(DestinationFilePath))
                .Returns(ParentDirectory);
            pathService
                .Setup(m => m.Combine(ParentDirectory, NewFileName))
                .Returns(NewFilePath);
            pathService
                .Setup(m => m.GetFileName(ParentDirectory))
                .Returns(ParentDirectoryName);

            var dialog = new OverwriteOptionsDialogViewModel(
                fileServiceMock.Object, fileSystemNodeViewModelFactory.Object,
                fileNameGenerationService.Object, pathService.Object);

            var parameter = new OverwriteOptionsNavigationParameter(SourceFilePath, DestinationFilePath, areMultipleFilesAvailable);
            dialog.Activate(parameter);

            return dialog;
        }
    }
}